using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using Redwerb.BizArk.Core.TypeExt;

namespace Redwerb.BizArk.Core.Convert
{
    /// <summary>
    /// Manages the strategies that can be used to convert values. Used by ConvertEx.
    /// </summary>
    public static class ConvertStrategyMgr
    {
        static ConvertStrategyMgr()
        {
            // Some types have commonly used aliases. These can be used for the ToXXX conversion methods.
            sTypeAliases = new Dictionary<Type, string[]>();
            sTypeAliases.Add(typeof(bool), new string[] { "Bool" });
            sTypeAliases.Add(typeof(int), new string[] { "Integer", "Int" });
            sTypeAliases.Add(typeof(short), new string[] { "Short" });
            sTypeAliases.Add(typeof(ushort), new string[] { "UShort" });
            sTypeAliases.Add(typeof(uint), new string[] { "UInt" });
            sTypeAliases.Add(typeof(long), new string[] { "Long" });
            sTypeAliases.Add(typeof(ulong), new string[] { "ULong" });
            sTypeAliases.Add(typeof(float), new string[] { "Float" });
        }

        private static Dictionary<Type, string[]> sTypeAliases;
        private static Dictionary<Type, Dictionary<Type, IConvertStrategy>> sStrategies = new Dictionary<Type, Dictionary<Type, IConvertStrategy>>();

        /// <summary>
        /// Gets a conversion strategy based on the from and to types.
        /// </summary>
        /// <param name="fromType"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        public static IConvertStrategy GetStrategy(Type fromType, Type toType)
        {
            IConvertStrategy strategy;

            Type fromKey = fromType ?? typeof(NullType);
            Type toKey = toType ?? typeof(NullType);

            // Check to see if the strategy already exists.
            if (sStrategies.ContainsKey(fromKey))
                if (sStrategies[fromKey].ContainsKey(toKey))
                {
                    strategy = sStrategies[fromType][toKey];
                    // A strategy can be set to null in which case
                    // we want to recreate it.
                    if (strategy != null)
                        return sStrategies[fromKey][toKey];
                }

            // The strategy has not been created yet. Create it now.

            strategy = CreateDefaultStrategy(fromType, toType);
            SetStrategy(fromType, toType, strategy);
            return strategy;
        }

        private static IConvertStrategy CreateDefaultStrategy(Type fromType, Type toType)
        {
            // Treat DBNull the same as null to make comparisons simpler.
            if (fromType == typeof(DBNull)) fromType = null;

            if (toType.IsAssignableFrom(fromType))
                return new NoConvertConversionStrategy();

            if (fromType == null && toType != null && toType.IsValueType)
                return new DefaultValueConversionStrategy(toType);

            if (fromType == null || toType == null)
                return new NullConversionStrategy();

            if (toType.IsEnum)
                return new EnumConversionStrategy(toType);

            if (fromType == typeof(string) && toType == typeof(bool))
                return new StringToBoolConversionStrategy();

            if (fromType == typeof(byte[]) && toType.IsDerivedFrom(typeof(Image)))
                return new ByteArrayToImageConversionStrategy();

            if (fromType.IsDerivedFrom(typeof(Image)) && toType == typeof(byte[]))
                return new ImageToByteArrayConversionStrategy();

            if (fromType == typeof(byte[]) && toType == typeof(string))
                return new ByteArrayToStringConversionStrategy(ASCIIEncoding.ASCII);

            if (fromType == typeof(string) && toType == typeof(byte[]))
                return new StringToByteArrayConversionStrategy(ASCIIEncoding.ASCII);

            TypeConverter converter = TypeDescriptor.GetConverter(fromType);
            if (converter != null && converter.CanConvertTo(toType))
                return new TypeConverterConversionStrategy(converter, toType);

            converter = TypeDescriptor.GetConverter(toType);
            if (converter != null && converter.CanConvertFrom(fromType))
                return new TypeConverterConversionStrategy(converter);

            MethodInfo mi;
            if (fromType == typeof(string))
            {
                mi = toType.GetMethod("Parse", new Type[] { typeof(string) });
                if (mi != null && mi.ReturnType == toType)
                    return new ParseConversionStrategy(mi);
            }

            mi = GetConvertMethod(fromType, toType);
            if (mi != null)
                return new ConvertMethodConversionStrategy(mi);

            var ci = GetCtor(fromType, toType);
            if (ci != null)
                return new CtorConversionStrategy(ci);

            if (fromType.Implements(typeof(IConvertible)) && ConvertibleConversionStrategy.CanConvertTo(toType))
                return new ConvertibleConversionStrategy(toType);

            if (fromType.IsDerivedFrom(typeof(Type)) && toType == typeof(SqlDbType))
                return new TypeToSqlDBTypeConversionStrategy();

            if (fromType.IsDerivedFrom(typeof(SqlDbType)) && toType == typeof(Type))
                return new SqlDBTypeToTypeConversionStrategy();

            return new InvalidConversionStrategy(String.Format("Invalid cast. Cannot convert from {0} to {1}.", fromType, toType));
        }

        private static MethodInfo GetConvertMethod(Type fromType, Type toType)
        {
            List<string> names = new List<string>();
            names.Add(toType.Name);
            if (sTypeAliases.ContainsKey(toType))
                names.AddRange(sTypeAliases[toType]);

            // Find the convert method.
            // Iterate through each of the alias' to find a method that
            // can be used to convert the value.
            foreach (string name in names)
            {
                MethodInfo mi = fromType.GetMethod("To" + name, new Type[] { });
                if (mi != null)
                    if (mi.ReturnType == toType)
                        return mi;
                    else
                        // If ToXXX is defined but returns the wrong type, the type
                        // can't define another ToXXX method with no parameters so 
                        // just return null now.
                        return null;
            }
            return null;
        }

        private static ConstructorInfo GetCtor(Type fromType, Type toType)
        {
            return toType.GetConstructor(new Type[] { fromType });
        }

        /// <summary>
        /// Sets the conversion strategy for converting from one type to another.
        /// </summary>
        /// <param name="fromType"></param>
        /// <param name="toType"></param>
        /// <param name="strategy"></param>
        public static void SetStrategy(Type fromType, Type toType, IConvertStrategy strategy)
        {
            Type fromKey = fromType ?? typeof(NullType);
            Type toKey = toType ?? typeof(NullType);

            if (!sStrategies.ContainsKey(fromKey))
                sStrategies.Add(fromKey, new Dictionary<Type, IConvertStrategy>());

            if (sStrategies[fromKey].ContainsKey(toKey))
                sStrategies[fromKey][toKey] = strategy;
            else
                sStrategies[fromKey].Add(toKey, strategy);
        }

        /// <summary>
        /// Used to represent a null Type. This class is never actually instantiated or used (we just use typeof())
        /// </summary>
        private static class NullType { }
    }
}
