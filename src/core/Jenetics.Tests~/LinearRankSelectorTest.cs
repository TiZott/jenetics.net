﻿// Java Genetic Algorithm Library.
// Copyright (c) 2017 Franz Wilhelmstötter
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Author:
//    Franz Wilhelmstötter (franz.wilhelmstoetter@gmx.at)

using System;
using System.Collections.Generic;
using System.Linq;
using Jenetics.Internal.Util;
using Jenetics.Stat;
using Jenetics.Util;
using Xunit;

namespace Jenetics
{
    public class LinearRankSelectorTest : ProbabilitySelectorTesterBase<LinearRankSelector<DoubleGene, double>>
    {
        protected override Factory<LinearRankSelector<DoubleGene, double>> Factory()
        {
            return () => new LinearRankSelector<DoubleGene, double>();
        }

        protected override bool IsSorted()
        {
            return true;
        }

        protected override LinearRankSelector<DoubleGene, double> Selector()
        {
            return new LinearRankSelector<DoubleGene, double>(0.0);
        }

        [Theory]
        [MemberData(nameof(ExpectedDistribution))]
        public void SelectDistribution(double nminus, Named<double[]> expected, Optimize opt)
        {
            Retry<Exception>(3, () =>
            {
                const int loops = 50;
                const int npopulation = PopulationCount;

                //ThreadLocal<LCG64ShiftRandom> random = new LCG64ShiftRandom.ThreadLocal();
                var random = RandomRegistry.GetRandom();
                RandomRegistry.Using(random, r =>
                {
                    var distribution = Distribution(
                        new LinearRankSelector<DoubleGene, double>(nminus),
                        opt,
                        npopulation,
                        loops
                    );

                    StatisticsAssert.AssertDistribution(distribution, expected.Value, 0.001, 5);
                });
            });
        }

        public static IEnumerable<object[]> ExpectedDistribution()
        {
            const string resource = "Jenetics.resources.LinearRankSelector";

            foreach (var opt in new[] {Optimize.Maximum, Optimize.Minimum})
            {
                var data = TestData.Of(resource, opt.ToString().ToUpper());

                var csv = data.Select(TestData.ToDouble).ToArray();

                for (var i = 0; i < csv[0].Length; i++)
                    yield return new object[]
                        {csv[0][i], Named<double[]>.Of($"distribution[{csv[0][i]}]", Expected(csv, i)), opt};
            }
        }

        private static double[] Expected(double[][] csv, int c)
        {
            var col = new double[csv.Length - 1];
            for (var i = 0; i < col.Length; ++i)
                col[i] = Math.Max(csv[i + 1][c], double.MinValue);
            return col;
        }
    }
}