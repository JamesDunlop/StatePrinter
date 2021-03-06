﻿// Copyright 2014 Kasper B. Graversen
// 
// Licensed to the Apache Software Foundation (ASF) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The ASF licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using System;
using NUnit.Framework;

using StatePrinter.OutputFormatters;

namespace StatePrinter.Tests.IntegrationTests
{
    [TestFixture]
    class ArrayTest
    {
        static readonly int[,] twoDimArray = { { 1, 2 }, { 3, 4 } };

        [TestFixture]
        class ArrayTestCurly
        {
            string expected = @"new Int32[,]()
[0] = 1
[1] = 2
[2] = 3
[3] = 4";

            [Test]
            public void TwoDimArray()
            {
                var printer = new Stateprinter();
                Assert.AreEqual(expected, printer.PrintObject(twoDimArray, ""));
            }

            [Test]
            public void TwoDimArray_LegacyApi()
            {
                var printer = new StatePrinter();
                printer.Configuration.LegacyBehaviour.TrimTrailingNewlines = false;
                Assert.AreEqual(expected + "\r\n", printer.PrintObject(twoDimArray, ""));
            }
        }


        [TestFixture]
        class ArrayTestJson
        {

            [Test]
            public void TwoDimArray()
            {
                var printer = TestHelper.CreateTestPrinter();
                printer.Configuration.SetOutputFormatter(new JsonStyle(printer.Configuration));

                string expected = @"
[
    1,
    2,
    3,
    4
]";
                printer.Assert.PrintEquals(expected, twoDimArray);
            }
        }

        [TestFixture]
        class ArrayTestXml
        {

            [Test]
            public void TwoDimArray()
            {
                var printer = TestHelper.CreateTestPrinter();
                printer.Configuration.SetOutputFormatter(new XmlStyle(printer.Configuration));

                string expected = @"<ROOT type='Int32[,]'>
    <Enumeration>
    <ROOT>1</ROOT>
    <ROOT>2</ROOT>
    <ROOT>3</ROOT>
    <ROOT>4</ROOT>
    </Enumeration>
</ROOT>";
                printer.Assert.PrintEquals(expected, twoDimArray);
            }
        }
    }
}
