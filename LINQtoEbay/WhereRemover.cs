using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace LINQtoEbay
{
    internal class WhereRemover : ExpressionVisitor
    {
        internal Expression RemoveWhere(Expression expr)
        {
            return this.Visit(expr);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.Name == "Where")
                return m.Arguments[0];
            else
                return base.VisitMethodCall(m);
        }
    }
}
