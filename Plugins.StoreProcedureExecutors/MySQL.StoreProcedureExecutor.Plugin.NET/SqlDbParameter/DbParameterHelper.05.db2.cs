#if IBM_Data_DB2_Core
namespace Microshaoft
{
    using Newtonsoft.Json.Linq;
    using System;
    using IBM.Data.DB2.Core;

    public static partial class DB2DbParameterHelper
    {
        public static object SetGetValueAsObject
                                    (
                                        this DB2Parameter @this
                                        , JToken jValue 
                                    )
        {
            object @return = null!;
            if
                (
                    jValue == null
                    ||
                    jValue.Type == JTokenType.Null
                    ||
                    jValue.Type == JTokenType.Undefined
                    ||
                    jValue.Type == JTokenType.None
                )
            {
                @return = DBNull.Value;
            }
            else
            {
                var jValueText = jValue.ToString();
                if
                    (
                        @this.DB2Type == DB2Type.LongVarChar
                        ||
                        @this.DB2Type == DB2Type.VarChar
                        ||
                        @this.DB2Type == DB2Type.NVarChar
                        ||
                        @this.DB2Type == DB2Type.Char
                        ||
                        @this.DB2Type == DB2Type.NChar
                    )
                {
                    @return = jValueText;
                }
                else if
                    (
                        @this.DB2Type == DB2Type.Date
                        ||
                        @this.DB2Type == DB2Type.DateTime
                        ||
                        @this.DB2Type == DB2Type.Time
                        ||
                        @this.DB2Type == DB2Type.Timestamp
                        ||
                        @this.DB2Type == DB2Type.TimeStampWithTimeZone
                    )
                {
                    if
                        (
                            DateTime
                                .TryParse
                                    (
                                        jValueText
                                        , out var @value
                                    )
                        )
                    {
                        @return = @value;
                    }
                }
                else if
                    (
                        @this.DB2Type == DB2Type.Boolean
                    )
                {
                    if
                        (
                            bool
                                .TryParse
                                    (
                                        jValueText
                                        , out var @value
                                    )
                        )
                    {
                        @return = @value;
                    }
                }
                else if
                    (
                        @this.DB2Type == DB2Type.Decimal
                    )
                {
                    if
                        (
                            decimal
                                .TryParse
                                    (
                                        jValueText
                                        , out var @value
                                    )
                        )
                    {
                        @return = @value;
                    }
                }
                else if
                    (
                        @this.DB2Type == DB2Type.Double
                        ||
                        @this.DB2Type == DB2Type.Real
                        ||
                        @this.DB2Type == DB2Type.Real370
                    )
                {
                    if
                        (
                            double
                                .TryParse
                                    (
                                        jValueText
                                        , out var @value
                                    )
                        )
                    {
                        @return = @value;
                    }
                }
                else if
                    (
                        @this.DB2Type == DB2Type.Float
                        ||
                        @this.DB2Type == DB2Type.SmallFloat
                        ||
                        @this.DB2Type == DB2Type.DecimalFloat
                    )
                {
                    if
                        (
                            float
                                .TryParse
                                    (
                                        jValueText
                                        , out var @value
                                    )
                        )
                    {
                        @return = @value;
                    }
                }
                else if
                    (
                        @this.DB2Type == DB2Type.Byte
                    )
                {
                    if
                        (
                            Guid
                                .TryParse
                                    (
                                        jValueText
                                        , out var @value
                                    )
                        )
                    {
                        @return = @value;
                    }
                }
                else if
                    (
                        @this.DB2Type == DB2Type.BigInt
                    )
                {
                    if
                        (
                            long
                                .TryParse
                                    (
                                        jValueText
                                        , out var @value
                                    )
                        )
                    {
                        @return = @value;
                    }
                }
                else if
                    (
                        @this.DB2Type == DB2Type.Integer
                    )
                {
                    if
                        (
                            int
                                .TryParse
                                    (
                                        jValueText
                                        , out var @value
                                    )
                        )
                    {
                        @return = @value;
                    }
                }
                else if
                    (
                        @this.DB2Type == DB2Type.SmallInt
                        ||
                        @this.DB2Type == DB2Type.Int8
                    )
                {
                    if
                        (
                            short
                                .TryParse
                                    (
                                        jValueText
                                        , out var @value
                                    )
                        )
                    {
                        @return = @value;
                    }
                }
            }
            return @return;
        }
    }
}
#endif