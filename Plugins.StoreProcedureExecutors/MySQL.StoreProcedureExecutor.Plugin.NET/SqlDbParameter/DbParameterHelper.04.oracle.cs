namespace Microshaoft
{
    using Newtonsoft.Json.Linq;
    using System;
    using Oracle.ManagedDataAccess.Client;
    public static partial class OracleDbParameterHelper
    {
        public static object SetGetValueAsObject
                                    (
                                        this OracleParameter @this
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
                        @this.OracleDbType == OracleDbType.Varchar2
                        ||
                        @this.OracleDbType == OracleDbType.NVarchar2
                        ||
                        @this.OracleDbType == OracleDbType.Char
                        ||
                        @this.OracleDbType == OracleDbType.NChar
                    )
                {
                    @return = jValueText;
                }
                else if
                    (
                        @this.OracleDbType == OracleDbType.Date
                        ||
                        @this.OracleDbType == OracleDbType.TimeStamp
                        ||
                        @this.OracleDbType == OracleDbType.TimeStampLTZ
                        ||
                        @this.OracleDbType == OracleDbType.TimeStampTZ
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
                        @this.OracleDbType == OracleDbType.Boolean
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
                        @this.OracleDbType == OracleDbType.Decimal
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
                        @this.OracleDbType == OracleDbType.Double
                    )
                {
                    var b = double
                                .TryParse
                                    (
                                        jValueText
                                        , out var @value
                                    );
                    if (b)
                    {
                        @return = @value;
                    }
                }
                else if
                    (
                        @this.OracleDbType == OracleDbType.Raw
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
                        @this.OracleDbType == OracleDbType.Long
                        ||
                        @this.OracleDbType == OracleDbType.Int64
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
                        @this.OracleDbType == OracleDbType.Int32
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
                        @this.OracleDbType == OracleDbType.Int16
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
