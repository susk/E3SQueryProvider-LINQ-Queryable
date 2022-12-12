using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Expressions.Task3.E3SQueryProvider
{
    public class ExpressionToFtsRequestTranslator : ExpressionVisitor
    {
        private readonly StringBuilder resultStringBuilder;

        public ExpressionToFtsRequestTranslator()
        {
            this.resultStringBuilder = new StringBuilder();
        }

        public string Translate(Expression exp)
        {
            this.Visit(exp);

            return this.resultStringBuilder.ToString();
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable)
                && node.Method.Name == "Where")
            {
                var predicate = node.Arguments[1];
                this.Visit(predicate);

                return node;
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    if (node.Left.NodeType != ExpressionType.MemberAccess)
                    {
                        throw new NotSupportedException($"Left operand should be property or field: {node.NodeType}");
                    }

                    if (node.Right.NodeType != ExpressionType.Constant)
                    {
                        throw new NotSupportedException($"Right operand should be constant: {node.NodeType}");
                    }

                    this.Visit(node.Left);
                    this.resultStringBuilder.Append("(");
                    this.Visit(node.Right);
                    this.resultStringBuilder.Append(")");
                    break;

                default:
                    throw new NotSupportedException($"Operation '{node.NodeType}' is not supported");
            }

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            this.resultStringBuilder.Append(node.Member.Name).Append(":");

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            this.resultStringBuilder.Append(node.Value);

            return node;
        }
    }
}
