using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using EventLogSearching.Model;

namespace EventLogSearching.Service
{
    public class SearchingExpressionBuilder
    {
        private static MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        private static MethodInfo toUpperMethod = typeof(string).GetMethod("ToUpper", new[] { typeof(string) });


        public static Expression<Func<T, bool>> GetExpression<T>(List<SearchElement> searchItems)
        {

            ParameterExpression pe = Expression.Parameter(typeof(T), "EventLog");

            Expression outerExp; //And between field

            outerExp = null;

            //Seperate Group Search
            try { 

                foreach (var searchItem in searchItems)
                {
                    Expression innerExp = null; //Or in the same field

                    innerExp = GetPriSearchExpression<T>(pe, searchItem);


                    if (innerExp != null && outerExp != null)
                    {
                        outerExp = PredicateBuilder.AndT(innerExp, outerExp);
                    }
                    else if (innerExp != null && outerExp == null)
                    {
                        outerExp = innerExp;
                    }
                    else if (innerExp == null && outerExp != null)
                    {
                        // Nothing to do
                    }
                    else
                    {
                        // Nothing to do
                    }


                } //next Group

                //Finishing
                return Expression.Lambda<Func<T, bool>>(outerExp, pe);
            }
            catch
            {
                return null;
            }
        }

        private static Expression GetPriSearchExpression<T>(ParameterExpression pe, SearchElement searchItem)
        {
            //List<Item> gruopfields = search_Parse_Pri_List.ToList();
            Expression innerExp = null; //Or in the same field
            //Expression outerExp = null; //Or in the same field

            if (searchItem.keyword.Length == 0)
                return null;
            //If it is SearchingField Group
            foreach (string keyWord in searchItem.keyword)
            {
                if (innerExp == null) //Start Building Expression
                {
                    innerExp = GetPriSearchExpression<T>(pe, searchItem.FieldName, keyWord);
                }
                else
                {
                    innerExp = Expression.OrElse(innerExp, GetPriSearchExpression<T>(pe, searchItem.FieldName, keyWord));
                }
            }
            return innerExp;
        }

        private static Expression GetPriSearchExpression<T>(ParameterExpression pe, string FieldName, string filter)
        {
            MemberExpression me = Expression.Property(pe, FieldName); //search in Field ?
            ConstantExpression constant = Expression.Constant(filter);
            return Expression.Call(me, containsMethod, constant);
        }
    }

    internal static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        public static Expression<Func<T, bool>> False<T>() { return f => false; }
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
        public static Expression AndT(this Expression expr1, Expression expr2)
        {
            return Expression.AndAlso(expr1, expr2);
        }


    }

}