/*
* Licensed to the Apache Software Foundation (ASF) under one or more
* contributor license agreements.  See the NOTICE file distributed with
* this work for additional information regarding copyright ownership.
* The ASF licenses this file to You under the Apache License, Version 2.0
* (the "License"); you may not use this file except in compliance with
* the License.  You may obtain a copy of the License at
*
*    http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using Apache.Arrow.Types;

namespace Apache.Arrow.Adbc.Tests.Drivers.BigQuery
{
    /// <summary>
    /// Gets the sample data
    /// </summary>
    internal class BigQueryData
    {
        /// <summary>
        /// Represents the different types of data for BigQuery.
        /// </summary>
        public static SampleDataBuilder GetSampleData()
        {
            Int64Array.Builder numbersBuilder = new Int64Array.Builder();
            numbersBuilder.AppendRange(new List<long>() { 1, 2, 3 });
            Int64Array numbersArray = numbersBuilder.Build();

            SampleDataBuilder sampleDataBuilder = new SampleDataBuilder();

            // standard values
            sampleDataBuilder.Samples.Add(
                new SampleData() {
                    Query = "SELECT " +
                            "CAST(1 as INT64) as id, " +
                            "CAST(1.23 as FLOAT64) as number, " +
                            "PARSE_NUMERIC(\"4.56\") as decimal, " +
                            "PARSE_BIGNUMERIC(\"7.89000000000000000000000000000000000000\") as big_decimal, " +
                            "CAST(True as BOOL) as is_active, " +
                            "'John Doe' as name, " +
                            "FROM_BASE64('YWJjMTIz') as data, " +
                            "CAST('2023-09-08' as DATE) as date, " +
                            "CAST('12:34:56' as TIME) as time, " +
                            "CAST('2023-09-08 12:34:56' as DATETIME) as datetime, " +
                            "CAST('2023-09-08 12:34:56+00:00' as TIMESTAMP) as timestamp, " +
                            "ST_GEOGPOINT(1, 2) as point, " +
                            "ARRAY[1, 2, 3] as numbers, " +
                            "STRUCT('John Doe' as name, 30 as age) as person",
                    ExpectedValues = new List<ColumnNetTypeArrowTypeValue>()
                    {
                        new ColumnNetTypeArrowTypeValue("id", typeof(long), typeof(Int64Type), 1L),
                        new ColumnNetTypeArrowTypeValue("number", typeof(double), typeof(DoubleType), 1.23d),
                        new ColumnNetTypeArrowTypeValue("decimal", typeof(SqlDecimal), typeof(Decimal128Type), SqlDecimal.Parse("4.56")),
                        new ColumnNetTypeArrowTypeValue("big_decimal", typeof(string), typeof(StringType), "7.89000000000000000000000000000000000000"),
                        new ColumnNetTypeArrowTypeValue("is_active", typeof(bool), typeof(BooleanType), true),
                        new ColumnNetTypeArrowTypeValue("name", typeof(string), typeof(StringType), "John Doe"),
                        new ColumnNetTypeArrowTypeValue("data", typeof(byte[]), typeof(BinaryType), UTF8Encoding.UTF8.GetBytes("abc123")),
                        new ColumnNetTypeArrowTypeValue("date", typeof(DateTime), typeof(Date64Type), new DateTime(2023, 9, 8)),
                        new ColumnNetTypeArrowTypeValue("time", typeof(long), typeof(Time64Type), 45296000000L), //'12:34:56'
                        new ColumnNetTypeArrowTypeValue("datetime", typeof(DateTimeOffset), typeof(TimestampType), new DateTimeOffset(new DateTime(2023, 9, 8, 12, 34, 56), TimeSpan.Zero)),
                        new ColumnNetTypeArrowTypeValue("timestamp", typeof(DateTimeOffset), typeof(TimestampType), new DateTimeOffset(new DateTime(2023, 9, 8, 12, 34, 56), TimeSpan.Zero)),
                        new ColumnNetTypeArrowTypeValue("point", typeof(string), typeof(StringType), "POINT(1 2)"),
                        new ColumnNetTypeArrowTypeValue("numbers", typeof(long), typeof(Int64Type), numbersArray),
                        new ColumnNetTypeArrowTypeValue("person", typeof(string), typeof(StringType), "{\"name\":\"John Doe\",\"age\":30}")
                    }
                });

            // big numbers
            sampleDataBuilder.Samples.Add(
                new SampleData()
                {
                    Query = "SELECT " +
                            "CAST(1.7976931348623157e+308 as FLOAT64) as number, " +
                            "PARSE_NUMERIC(\"9.99999999999999999999999999999999E+28\") as decimal, " +
                            "PARSE_BIGNUMERIC(\"5.7896044618658097711785492504343953926634992332820282019728792003956564819968E+37\") as big_decimal ",
                    ExpectedValues = new List<ColumnNetTypeArrowTypeValue>()
                    {
                        new ColumnNetTypeArrowTypeValue("number", typeof(double), typeof(DoubleType), 1.7976931348623157e+308d),
                        new ColumnNetTypeArrowTypeValue("decimal", typeof(SqlDecimal), typeof(Decimal128Type), SqlDecimal.Parse("99999999999999999999999999999.999900000")),
                        new ColumnNetTypeArrowTypeValue("big_decimal", typeof(string), typeof(StringType), "57896044618658097711785492504343953926.63499233282028201972879200395656481997")
                    }
                });

            // nulls
            Int64Array.Builder emptyNumbersBuilder = new Int64Array.Builder();
            Int64Array emptyNumbersArray = emptyNumbersBuilder.Build();

            sampleDataBuilder.Samples.Add(
                new SampleData()
                {
                    Query = "SELECT  " +
                            "CAST(NULL as INT64) as id, " +
                            "CAST(NULL as FLOAT64) as number, " +
                            "PARSE_NUMERIC(NULL) as decimal,  " +
                            "PARSE_BIGNUMERIC(NULL) as big_decimal, " +
                            "CAST(NULL as BOOL) as is_active, " +
                            "CAST(NULL as STRING)  as name, " +
                            "FROM_BASE64(NULL) as data, " +
                            "CAST(NULL as DATE) as date, " +
                            "CAST(NULL as TIME) as time, " +
                            "CAST(NULL as DATETIME) as datetime, " +
                            "CAST(NULL as TIMESTAMP) as timestamp," +
                            "ST_GEOGPOINT(NULL, NULL) as point, " +
                            "CAST(NULL as ARRAY<INT64>) as numbers, " +
                            "STRUCT(CAST(NULL as STRING) as name, CAST(NULL as INT64) as age) as person",
                    ExpectedValues = new List<ColumnNetTypeArrowTypeValue>()
                    {
                        new ColumnNetTypeArrowTypeValue("id", typeof(long), typeof(Int64Type), null),
                        new ColumnNetTypeArrowTypeValue("number", typeof(double), typeof(DoubleType), null),
                        new ColumnNetTypeArrowTypeValue("decimal", typeof(SqlDecimal), typeof(Decimal128Type), null),
                        new ColumnNetTypeArrowTypeValue("big_decimal", typeof(string), typeof(StringType), null),
                        new ColumnNetTypeArrowTypeValue("is_active", typeof(bool), typeof(BooleanType), null),
                        new ColumnNetTypeArrowTypeValue("name", typeof(string), typeof(StringType), null),
                        new ColumnNetTypeArrowTypeValue("data", typeof(byte[]), typeof(BinaryType), null),
                        new ColumnNetTypeArrowTypeValue("date", typeof(DateTime), typeof(Date64Type), null),
                        new ColumnNetTypeArrowTypeValue("time", typeof(long), typeof(Time64Type), null),
                        new ColumnNetTypeArrowTypeValue("datetime", typeof(DateTimeOffset), typeof(TimestampType), null),
                        new ColumnNetTypeArrowTypeValue("timestamp", typeof(DateTimeOffset), typeof(TimestampType), null),
                        new ColumnNetTypeArrowTypeValue("point", typeof(string), typeof(StringType), null),
                        new ColumnNetTypeArrowTypeValue("numbers", typeof(long), typeof(Int64Type), emptyNumbersArray),
                        new ColumnNetTypeArrowTypeValue("person", typeof(string), typeof(StringType), "{\"name\":null,\"age\":null}")
                    }
                });

            return sampleDataBuilder;
        }
    }
}
