using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Models
{
    public class StringTrimmerInterceptor : IDbCommandTreeInterceptor
    {
        private static readonly string[] _typesToTrim = {
            "nvarchar",
            "varchar",
            "char",
            "nchar"
        };
        public void TreeCreated(DbCommandTreeInterceptionContext interceptionContext)
        {
            if (interceptionContext.OriginalResult.DataSpace == DataSpace.SSpace)
            {
                var insertCommand = interceptionContext.Result as DbInsertCommandTree;
                if (insertCommand != null)
                {

                    List<DbModificationClause> finalSetClauses =
                        new List<DbModificationClause>(
                            (IEnumerable<DbModificationClause>)insertCommand.SetClauses.Select(
                                a => {
                                    var dbSetClause = a as DbSetClause;
                                    if (dbSetClause != null)
                                    {
                                        var dbPropertyExpression = dbSetClause.Property as DbPropertyExpression;

                                        if (dbPropertyExpression != null)
                                        {
                                            var edmProperty = dbPropertyExpression.Property as EdmProperty;
                                            if (edmProperty != null && edmProperty.MaxLength != null &&
                                                _typesToTrim.Contains(edmProperty.TypeName))
                                            {
                                                var dbConstantExpression = dbSetClause.Value as DbConstantExpression;
                                                if (dbConstantExpression != null && dbConstantExpression.Value != null)
                                                {
                                                    var value = dbConstantExpression.Value.ToString();
                                                    if (!string.IsNullOrEmpty(value) &&
                                                        value.Length > (int)edmProperty.MaxLength)
                                                    {
                                                        return DbExpressionBuilder.SetClause(
                                                            DbExpressionBuilder.Property(
                                                                DbExpressionBuilder.Variable(
                                                                    insertCommand.Target.VariableType,
                                                                    insertCommand.Target.VariableName),
                                                                dbPropertyExpression.Property.Name),
                                                            EdmFunctions.Trim(
                                                                dbConstantExpression.Value.ToString()
                                                                .Substring(0, (int)edmProperty.MaxLength)));
                                                    }

                                                }
                                            }
                                        }
                                    }

                                    return a;
                                }));

                    var newInsertCommand = new DbInsertCommandTree(
                        insertCommand.MetadataWorkspace,
                        insertCommand.DataSpace,
                        insertCommand.Target,
                        new ReadOnlyCollection<DbModificationClause>(finalSetClauses),
                        insertCommand.Returning);

                    interceptionContext.Result = newInsertCommand;
                }
            }

            var updateCommand = interceptionContext.Result as DbUpdateCommandTree;
            if (updateCommand != null)
            {

                List<DbModificationClause> finalSetClauses =
                    new List<DbModificationClause>(
                        (IEnumerable<DbModificationClause>)updateCommand.SetClauses.Select(
                            a => {
                                var dbSetClause = a as DbSetClause;
                                if (dbSetClause != null)
                                {
                                    var dbPropertyExpression = dbSetClause.Property as DbPropertyExpression;

                                    if (dbPropertyExpression != null)
                                    {
                                        var edmProperty = dbPropertyExpression.Property as EdmProperty;
                                        if (edmProperty != null && edmProperty.MaxLength != null &&
                                            _typesToTrim.Contains(edmProperty.TypeName))
                                        {
                                            var dbConstantExpression = dbSetClause.Value as DbConstantExpression;
                                            if (dbConstantExpression != null && dbConstantExpression.Value != null)
                                            {
                                                var value = dbConstantExpression.Value.ToString();
                                                if (!string.IsNullOrEmpty(value) &&
                                                    value.Length > (int)edmProperty.MaxLength)
                                                {
                                                    return DbExpressionBuilder.SetClause(
                                                        DbExpressionBuilder.Property(
                                                            DbExpressionBuilder.Variable(
                                                                updateCommand.Target.VariableType,
                                                                updateCommand.Target.VariableName),
                                                            dbPropertyExpression.Property.Name),
                                                        EdmFunctions.Trim(
                                                            dbConstantExpression.Value.ToString()
                                                            .Substring(0, (int)edmProperty.MaxLength)));
                                                }

                                            }
                                        }
                                    }
                                }

                                return a;
                            }));

                var newInsertCommand = new DbUpdateCommandTree(
                    updateCommand.MetadataWorkspace,
                    updateCommand.DataSpace,
                    updateCommand.Target,
                    updateCommand.Predicate,
                    new ReadOnlyCollection<DbModificationClause>(finalSetClauses),
                    updateCommand.Returning);

                interceptionContext.Result = newInsertCommand;
            }
        }
    }
    
}