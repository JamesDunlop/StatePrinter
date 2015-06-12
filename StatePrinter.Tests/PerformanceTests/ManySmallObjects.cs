﻿// Copyright 2014-2015 Kasper B. Graversen
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
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using StatePrinter.Configurations;
using StatePrinter.OutputFormatters;

namespace StatePrinter.Tests.PerformanceTests
{
    [TestFixture]
    [Explicit]
    internal class ManySmallObjects
    {
        const int N = 1000000;


        /// <summary>
        /// printing many times reveals the overhead of starting a print
        /// 
        /// Version 1.04 - HPPavilion 7
        /// 11941
        /// 
        /// Version 2.00 - HPPavilion 7
        /// 9506
        /// 
        /// Version 2.20 - HPPavilion 7
        /// 5206
        /// </summary>
        [Test]
        public void TestManyPrintings()
        {
            var toPrint = new Base();
            var printer = new Stateprinter();

            printer.PrintObject(toPrint);
            var mills = Time(
              () =>
              {
                  for (int i = 0; i < N; i++)
                      printer.PrintObject(toPrint);
              });
            Console.WriteLine("  " + mills);
        }


        /// <summary>
        /// Many small objects reveals the overhead of introspecting types.
        /// 
        /// Version 1.04 - HPPavilion 7
        /// 1000:  130
        /// 2000:  29
        /// 4000:  56
        /// 8000:  122
        /// 16000:  259
        /// 32000:  497
        /// 64000:  1021
        /// 128000:  2062
        /// 256000:  4332
        /// 512000:  8567
        /// 1024000:  16878
        /// 
        /// 
        /// Version 2.00 - HPPavilion 7
        /// 1000:  132
        /// 2000:  26
        /// 4000:  58
        /// 8000:  122
        /// 16000:  257
        /// 32000:  510
        /// 64000:  1022
        /// 128000:  2064
        /// 256000:  4220
        /// 512000:  8426
        /// 1024000:  16904
        /// 
        /// 
        /// Version 2.2 - HPPavilion 7
        /// 1000:  158
        /// 2000:  8
        /// 4000:  19
        /// 8000:  50
        /// 16000:  86
        /// 32000:  175
        /// 64000:  371
        /// 128000:  781
        /// 256000:  1590
        /// 512000:  3217
        /// 1024000:  6265
        /// </summary>
        [Test]
        public void DumpManySmallObjects()
        {
            for (int i = 1000; i <= N * 2; i *= 2)
            {
                DumpNObjects(i);
            }
        }

        private void DumpNObjects(int max)
        {
            var x = new List<ToDump>();
            for (int i = 0; i < max; i++)
                x.Add(new ToDump());

            var cfg = ConfigurationHelper.GetStandardConfiguration();
            cfg.OutputFormatter = new JsonStyle(cfg);
            var mills = Time(() =>
                             {
                                 var printer = new Stateprinter(cfg);
                                 printer.PrintObject(x);
                             });
            Console.WriteLine(max + ":  " + mills);
        }


        internal class Base
        {
            internal string Boo = null;
        }


        private class ToDump : Base
        {
            internal string Poo = "dd";
        }


        private long Time(Action a)
        {
            var watch = new Stopwatch();
            watch.Start();
            a();
            watch.Stop();
            return watch.ElapsedMilliseconds;
        }
    }
}