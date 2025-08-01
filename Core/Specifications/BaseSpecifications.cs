﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

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

        // Construtores
        public BaseSpecification ( ) { }

        public BaseSpecification ( Expression<Func<T, bool>> criteria )
        {
            Criteria = criteria;
        }

        // Includes
        public void AddInclude ( Expression<Func<T, object>> includeExpression )
        {
            Includes.Add ( includeExpression );
        }

        protected void AddInclude ( string includeString )
        {
            IncludeStrings.Add ( includeString );
        }

        // Ordenação
        protected void AddOrderBy ( Expression<Func<T, object>> orderByExpression )
        {
            OrderBy = orderByExpression;
        }

        protected void AddOrderByDescending ( Expression<Func<T, object>> orderByDescendingExpression )
        {
            OrderByDescending = orderByDescendingExpression;
        }

        // Paginação
        protected void ApplyPaging ( int skip, int take )
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }




        protected void ApplySorting ( string? sort )
        {
            if ( string.IsNullOrWhiteSpace ( sort ) )
            {
                // Determina a propriedade de data padrão baseada no tipo
                var defaultDateProperty = typeof(T).Name switch
                {
                    "Payment" => "PaymentDate",
                    "ServiceOrder" => "DateIn",
                    "ProductionStage" => "DateIn",
                    _ => "CreatedAt"
                };

                AddOrderByDescending ( x => EF.Property<object> ( x!, defaultDateProperty ) );
                return;
            }

            var descending = sort.EndsWith("Desc", StringComparison.OrdinalIgnoreCase);
            var propertyName = descending
        ? sort[..^4] // remove "Desc"
        : sort;

            var propInfo = typeof(T).GetProperties()
    .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));
            if ( propInfo == null )
            {

                var defaultDateProperty = typeof(T).Name switch
                {
                    "Payment" => "PaymentDate",
                    "ServiceOrder" => "DateIn",
                    "ProductionStage" => "DateIn",
                    _ => "CreatedAt"
                };

                AddOrderByDescending ( x => EF.Property<object> ( x!, defaultDateProperty ) );
                return;
            }

            if ( descending )
                AddOrderByDescending ( x => EF.Property<object> ( x!, propInfo.Name ) );
            else
                AddOrderBy ( x => EF.Property<object> ( x!, propInfo.Name ) );
        }


    }
}
