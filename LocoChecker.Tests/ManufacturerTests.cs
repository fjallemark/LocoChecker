﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tellurian.Trains.Modules.Tests;

[TestClass]
public class ManufacturerTests
{
    [TestMethod]
    public void LoadsAllFromFile()
    {
        var result = "Manufacturers.txt".Read();
        Assert.AreEqual(161, result.Count());
    }
}
