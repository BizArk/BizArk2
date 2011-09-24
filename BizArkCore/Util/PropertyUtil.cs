using System;
using System.Linq.Expressions;
using System.Reflection;

namespace BizArk.Core.Util
{

    /// <summary>
    /// Provides methods that are useful when working with properties.
    /// </summary>
    public static class PropertyUtil
    {

        /// <summary>
        /// Gets the name of the property based on a Linq expression.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="propertyRefExpr"></param>
        /// <returns></returns>
        public static string GetName<TObject>(Expression<Func<TObject, object>> propertyRefExpr)
        {
            return GetNameCore(propertyRefExpr.Body);
        }

        /// <summary>
        /// Gets the name of the property in the expression.
        /// </summary>
        /// <param name="propertyRefExpr"></param>
        /// <returns></returns>
        public static string GetNameCore(Expression propertyRefExpr)
        {
            if (propertyRefExpr == null)
                throw new ArgumentNullException("propertyRefExpr", "propertyRefExpr is null.");

            var memberExpr = propertyRefExpr as MemberExpression;
            if (memberExpr == null)
            {
                var unaryExpr = propertyRefExpr as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                    memberExpr = unaryExpr.Operand as MemberExpression;
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
                return memberExpr.Member.Name;

            throw new ArgumentException("No property reference expression was found.", "propertyRefExpr");
        }

    }
}
