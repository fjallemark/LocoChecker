# Loco Checker
Package for validating DCC decoders at module meetings. 

## Background
At model railway module meetings locos are often supplied by different participants.
Therefore, loco adresses has to be coordinated.

Within FREMO, there exists a numbering scheme to avoid clashing adresses. 
Other meetings have a local list where participats book numbers.
Unless each loco is checked by someone before put on track, there are no guarantees
that adresses are clashing and the other rules below are enforced.

The *Loco Checker* is a component to be used in a self-service registration kiosk,
where participants register locos before they are put on track. 
The *Loco Checker* adjusts the loco configuration and register the loco address in a 
*Loco Registry Service*. That data can  then used by another service to monitor what adresses that are used
in the throttle control bus, and non-registerde addresses will be blocked by always
re-setting speed to zero.


## Rules
1. Loco addresses must be unique for all locos at the meeting. 
Address 3 must not be used because it is often manufacturer default.
Loco address uniquness is validated against a *loco registery service*.
2. If the loco is operated by other than its owner, deacceleration time must be turned off or
very short. If its not possible to disabled long deacceleration with the throttle, CV4 is adjusted to 1.
3. Analog operation is not permitted because booster failure may cause locos to rush uncontrollable.
Therefore analog operation is turned off.
4. If RailCom® is enabled, it is turned off.
## Features
The original loco configuration can be stored. When a loco is read again, 
the loco owner has an option to restore the loco settings as it was upon arriving to the meeting. 
This also works if the loco address tempoarily was changed during the meeting using this module.

If the loco address is occupied by another participant, it is possible to temporarily change the address.

If decoder number CV16 is non-zero, the same value will be written to CV15 before any other CV's are written.
After all CV's are written, CV15 is zeroed again. 

If Consist Address CV19 is non-zero, it is detected and can be corrected if not intended.

## Limitations
* Only DCC decoders are supported.
* Only locos with single decoder installed. 

## Locomotive programming station
In order to relieve technical staff at module meetings, one way is to increase the degree of self-service. 
Below is described how the participants themselves could register their locomotives, 
get them programmed correctly and be able to restore the locomotives to their original settings at the end of the meeting.
### Functions
#### Register locomotives
The user places the locomotive on the programming track.
Read locomotive is selected on the screen.
The following CV is read by:
- 1: short address
- 4: rollout
- 7: decoder version
- 8: decoder make
- 17 and 18: long address
- 19: composition address
- 29: configuration

The locomotive address is determined by:
- 1. if bit 5 of CV29 is 0, then CV1 is the address.
- 2. otherwise CV17/18 is the address.
- 3. If CV19 > 0, then CV19 is an additional address.

The locomotive address is looked up in the notification register and the following information is retrieved if it exists:
- Owner's name
- Operator signature
- Vhicle class
- Vehicle number

The data is presented to the user with the following comments:
- Current local address is displayed.
- If the address or additional address is in the notification register, the user must first confirm that it is the correct owner's name: Is it your vehicle?
- If NO: The user must be prompted to change the locomotive address and get a suggestion for a free address, as well as fill in their name and the locomotive operator's signature, letter and number.
- If the address is missing from the notification register, the user must fill in his name and the locomotive's operator signature, letter and number.
- If CV19 > 0, the user must be informed of this and given the opportunity to reset it to zero.
- Ask if the locomotive is to be used by other participants? IF YES: If CV4 is greater than a configurable value, then the user must be informed that the rollout is to high and must be set to the configured value.
- If bit 2 in CV29 is 1, the user must be informed that analog operation will be switched off.
- Inform about the number of speed steps: if bit 1 in CV29 = 1, there are 14 speed steps, otherwise they are 28 or 128 speed steps.
- User clicks Accept.

#### Restore the locomotive
Original settings for CV 1,4,17,18,19,29 are saved under the address the locomotive has after the programming is done. 
This allows you to restore the locomotive to the settings the locomotive had when it was read the first time, e.g. before going home from the meeting.

### Solution
I've been thinking of using the Z21.
A piece of track is connected to the programming track output. 
The track is bounded at both ends by stable blocks so that locomotives do not risk hitting the floor if they roll away during reading and programming.
An information sign shows how to start the app on your mobile phone. 
As a supplement, you can also have a computer or tablet on site. The app should work as above.
The app communicates with a web service which in turn communicates with the Z21's functions to read and write resumes on the programming track.

The program code should enable implemetations to read and write CV to other types of command stations.

#### Special cases
You may need to take into account the number of speed steps.

*Determines the decoder's braking rate.  The formula for the braking rate shall be equal to (the contents of CV#3*.896)/(number of speed steps in use).
*For example, if the contents of CV#3 =2, then the braking is 0.064 sec/step for a decoder currently using 28 speed steps.
If the content of this parameter equals "0" then there is no programmed momentum during acceleration.*

Note that the NMRA standard does not require the 0.896 factor to be used, e.g. ESU has completely different lower values:

*Uniform Spec: A "Y" in the Uniform Spec column indicates a CV which requires implementation by manufacturers according to a common specification.
A blank in the Uniform Specification means that the CV must be used for its designated purpose,
but the action taken by the decoder for a specific value can vary from manufacturer to manufacturers.*


