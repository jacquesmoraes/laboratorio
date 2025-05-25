using System.Linq.Expressions;

namespace Core.Specifications
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>>? Criteria { get; private set; }
        public List<Expression<Func<T, object>>> Includes { get; } = [];
        public List<string> IncludeStrings { get; } = [];
        public Expression<Func<T, object>>? OrderBy { get; private set; }
        public Expression<Func<T, object>>? OrderByDescending { get; private set; }
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; }

        public BaseSpecification ( ) { }

        public BaseSpecification ( Expression<Func<T, bool>> criteria )
        {
            Criteria = criteria;
        }

        protected void AddInclude ( Expression<Func<T, object>> includeExpression )
        {
            Includes.Add ( includeExpression );
        }
         protected void AddInclude ( string includeString )
        {
            IncludeStrings.Add ( includeString );
        }
        protected void AddOrderBy ( Expression<Func<T, object>> orderByExpression )
        {
            OrderBy = orderByExpression;
        }

        protected void AddOrderByDescending ( Expression<Func<T, object>> orderByDescendingExpression )
        {
            OrderByDescending = orderByDescendingExpression;
        }

        protected void ApplyPaging ( int skip, int take )
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

       
    }
}
