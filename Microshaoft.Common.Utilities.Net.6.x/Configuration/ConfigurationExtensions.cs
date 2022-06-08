namespace Microshaoft
{
    using Microsoft.Extensions.Configuration;
    using System.Diagnostics;

    public static class ConfigurationExtensions
    {
        public static bool TryGetSection
                      (
                          this IConfiguration @this
                          , string sectionKey
                          , out IConfigurationSection section
                      )
        {
            if (sectionKey == null)
            {
                section = (IConfigurationSection)@this;
            }
            else
            {
                section = @this
                                .GetSection
                                    (
                                        sectionKey
                                    );
            }
            var r = section.Exists();
            if (!r)
            {
                section = null!;
            }
            return r;
        }
        // only for Array Value
        public static T GetOrDefault<T>
                            (
                                this IConfiguration @this
                                , string sectionKey
                                , T defaultValue = default!
                            )
        {
            T r = defaultValue;
            var b = TryGet
                        (
                            @this
                            , sectionKey
                            , out T @value
                        );
            if (b)
            {
                r = @value;
            }
            return r;
        }


        public static bool TryGet<T>
                        (
                            this IConfiguration @this
                            , string sectionKey
                            , out T sectionValue
                        )
        {
            var r = TryGetSection
                        (
                            @this
                            , sectionKey
                            , out var configuration
                        );
            if (r)
            {
                r = false;
                sectionValue = configuration
                                        .Get<T>();
                r = true;
            }
            else
            {
                sectionValue = default!;
            }
            return r;
        }

        //2022-06-08 .NET 6.0
        public static T Get<T>
                (
                    this IConfigurationSection @this
                )
        {
            T @return = default!;
            Type t = typeof(T);

            //@this


            if (@this.Value != null)
            {
                //if (typeof(T) != typeof(Array))
                string sectionValueText = @this.Value;
                if (t.IsArray)
                {
                    var elementType = t.GetElementType();
                    if (typeof(string).IsAssignableFrom(elementType))
                    {
                        @return = (T)Convert.ChangeType(sectionValueText, t);

                    }
                    else if (typeof(int).IsAssignableFrom(elementType))
                    { 
                    
                    
                    }


                    
                    Console.ReadLine();
                
                }
                else if (t == typeof(string))
                {
                    @return = (T)Convert.ChangeType(sectionValueText, t);
                }
                else if (t == typeof(int))
                {
                    if (int.TryParse(sectionValueText, out int @out))
                    {
                        @return = (T)Convert.ChangeType(@out, t);
                    }
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    if (DateTime.TryParse(sectionValueText, out DateTime @out))
                    {
                        @return = (T)Convert.ChangeType(@out, t);
                    }
                }
                else if (typeof(T) == typeof(DateOnly))
                {
                    if (DateOnly.TryParse(sectionValueText, out DateOnly @out))
                    {
                        @return = (T)Convert.ChangeType(@out, t);
                    }
                }
            }
            return @return;

        }

    }
}