namespace Microshaoft
{
    using Newtonsoft.Json.Linq;
    using Npgsql;
    using NpgsqlTypes;
    using System;
    public static partial class NpgsqlDbParameterHelper
    {
        public static object SetGetValueAsObject
                                    (
                                        this NpgsqlParameter @this
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
                    @this.NpgsqlDbType == NpgsqlDbType.Varchar
                    ||
                    @this.NpgsqlDbType == NpgsqlDbType.Text
                    ||
                    @this.NpgsqlDbType == NpgsqlDbType.Char
                )
                {
                    @return = jValueText;
                }
                else if
                    (
                        @this.NpgsqlDbType == NpgsqlDbType.Date
                        ||
                        @this.NpgsqlDbType == NpgsqlDbType.Time
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
                        @this.NpgsqlDbType == NpgsqlDbType.Bit
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
                        @this.NpgsqlDbType == NpgsqlDbType.Double
                        ||
                        @this.NpgsqlDbType == NpgsqlDbType.Real
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
                        @this.NpgsqlDbType == NpgsqlDbType.Uuid
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
                        @this.NpgsqlDbType == NpgsqlDbType.Integer
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
                        @this.NpgsqlDbType == NpgsqlDbType.Bigint
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
                        @this.NpgsqlDbType == NpgsqlDbType.Numeric
                   )
                {
                    var b = decimal
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
            }
            return @return;
        }
    }
}