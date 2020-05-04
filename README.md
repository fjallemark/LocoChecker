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

