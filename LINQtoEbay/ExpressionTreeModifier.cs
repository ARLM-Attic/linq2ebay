using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LINQtoEbay
{
    internal class ExpressionTreeModifier : ExpressionVisitor
    {
        private IQueryable<EbayItem> queryableItems;

        internal ExpressionTreeModifier(IQueryable<EbayItem> queryableItems)
        {
            this.queryableItems = queryableItems;
        }

        internal Expression CopyAndModify(Expression expression)
        {
            return this.Visit(expression);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Type == typeof(EbayQuery<EbayItem>))
                return Expression.Constant(this.queryableItems);
            else
                return c;
        }
    }
}
