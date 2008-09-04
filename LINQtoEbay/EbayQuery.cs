using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LINQtoEbay
{
    public class EbayQuery<T> : IOrderedQueryable<T>
    {
        public EbayQuery()
        {
            Provider = new EbayQueryProvider();
            Expression = Expression.Constant(this);
        }

        public EbayQuery(IQueryProvider provider, Expression expression)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (expression == null)
                throw new ArgumentNullException("expression");

            Provider = provider;
            Expression = expression;
        }

        public Expression Expression { get; private set; }

        public IQueryProvider Provider { get; private set; }

        public Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (Provider.Execute<IEnumerable<T>>(Expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (Provider.Execute<IEnumerable>(Expression)).GetEnumerator();
        }
    }
}