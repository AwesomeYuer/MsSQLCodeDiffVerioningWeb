namespace Microshaoft
{
    using MySql.Data.MySqlClient;
    using Newtonsoft.Json.Linq;
    using System;
    public static partial class MySqlDbParameterHelper
    {
        public static object SetGetValueAsObject
                                    (
                                        this MySqlParameter @this
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
                        @this.MySqlDbType == MySqlDbType.VarChar
                        ||
                        @this.MySqlDbType == MySqlDbType.Text
                        ||
                        @this.MySqlDbType == MySqlDbType.VarString
                    )
                {
                    @return = jValueText;
                }
                else if
                    (
                        @this.MySqlDbType == MySqlDbType.DateTime
                        ||
                        @this.MySqlDbType == MySqlDbType.Date
                        ||
                        @this.MySqlDbType == MySqlDbType.DateTime
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
                        @this.MySqlDbType == MySqlDbType.Bit
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
                        @this.MySqlDbType == MySqlDbType.Decimal
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
                        @this.MySqlDbType == MySqlDbType.Float
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
                        @this.MySqlDbType == MySqlDbType.Guid
                    )
                {
                    if
                        (
                            Guid
                                .TryParse
                                    (
                                        jValueText
                                        , out Guid @value
                                    )
                        )
                    {
                        @return = @value;
                    }
                }
                else if
                    (
                        @this.MySqlDbType == MySqlDbType.UInt16
                    )
                {
                    if
                        (
                            ushort
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
                        @this.MySqlDbType == MySqlDbType.UInt24
                        ||
                        @this.MySqlDbType == MySqlDbType.UInt32
                    )
                {
                    if
                        (
                            uint
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
                        @this.MySqlDbType == MySqlDbType.UInt64
                    )
                {
                    if
                        (
                            ulong
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
                       @this.MySqlDbType == MySqlDbType.Int16
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
                else if
                   (
                        @this.MySqlDbType == MySqlDbType.Int24
                        ||
                        @this.MySqlDbType == MySqlDbType.Int32
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
                        @this.MySqlDbType == MySqlDbType.Int64
                   )
                {
                    if
                        (
                            long
                                .TryParse
                                    (
                                        jValueText
                                        , out long @value
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
