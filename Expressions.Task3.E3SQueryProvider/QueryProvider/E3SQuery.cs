using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Expressions.Task3.E3SQueryProvider.QueryProvider
{
    public class E3SQuery<T> : IQueryable<T>
    {
        private readonly E3SLinqProvider provider;

        public E3SQuery(Expression expression, E3SLinqProvider provider)
        {
            this.Expression = expression;
            this.provider = provider;
        }

        public Type ElementType => typeof(T);

        public Expression Expression { get; }

        public IQueryProvider Provider => this.provider;

        public IEnumerator<T> GetEnumerator()
        {
            return this.provider.Execute<IEnumerable<T>>(this.Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.provider.Execute<IEnumerable>(this.Expression).GetEnumerator();
        }
    }
}
