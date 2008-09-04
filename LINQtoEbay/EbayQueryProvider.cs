using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace LINQtoEbay
{
    internal class EbayQueryProvider : IQueryProvider
    {
        IQueryable<T> IQueryProvider.CreateQuery<T>(Expression expression)
        {
            return new EbayQuery<T>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(EbayQuery<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public T Execute<T>(Expression expression)
        {
            bool IsEnumerable = (typeof(T).Name == "IEnumerable`1");

            return (T)Parse(expression, IsEnumerable);
        }

        public object Execute(Expression expression)
        {
            return Parse(expression, false);
        }

        private object Parse(Expression expression, bool isEnumerable)
        {
            InnermostWhereFinder whereFinder = new InnermostWhereFinder();
            MethodCallExpression whereExpression = whereFinder.GetInnermostWhere(expression);

            if (whereExpression == null)
                throw new Exception("Query doesn't contain a where clause");

            LambdaExpression lambdaExpression = (LambdaExpression)((UnaryExpression)(whereExpression.Arguments[1])).Operand;

            lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            EbayItem[] items = EbayHelper.PerformWebQuery(new EbayExpressionVisitor(lambdaExpression.Body).ProcessExpression());

            IQueryable<EbayItem> query = items.AsQueryable<EbayItem>();
            ExpressionTreeModifier modifier = new ExpressionTreeModifier(query);
            Expression newExpr = modifier.CopyAndModify(expression);
            newExpr = new WhereRemover().RemoveWhere(newExpr);

            if (isEnumerable)
                return query.Provider.CreateQuery(newExpr);
            else
                return query.Provider.Execute(newExpr);
        }
    }
}
