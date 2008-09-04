using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace LINQtoEbay
{
    internal class EbayExpressionVisitor : ExpressionVisitor
    {
        EbaySearchCriteria criteria;
        Expression expression;

        internal EbayExpressionVisitor(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            this.expression = expression;
        }

        internal EbaySearchCriteria ProcessExpression()
        {
            criteria = new EbaySearchCriteria();
            this.Visit(expression);

            return criteria;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.Equal:
                    return VisitEqual(b);
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    return VisitGreater(b);
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    return VisitLess(b);
            }

            return base.VisitBinary(b);
        }

        private Expression VisitEqual(BinaryExpression expression)
        {
            object val;

            if (expression.Left.NodeType == ExpressionType.MemberAccess || expression.Right.NodeType == ExpressionType.MemberAccess)
            {
                /*if (expression.Right.NodeType == ExpressionType.Constant)
                    val = ((ConstantExpression)expression.Right).Value;
                else if (expression.Right.NodeType == ExpressionType.MemberAccess)
                    val = GetMemberValue((MemberExpression)expression.Right);
                else
                    throw new NotSupportedException();*/

                if (expression.Right.NodeType == ExpressionType.Constant)
                    val = ((ConstantExpression)expression.Right).Value;
                else
                    val = ((ConstantExpression)expression.Left).Value;

                switch (((MemberExpression)expression.Left).Member.Name)
                {
                    case "BuyItNowPrice":
                        criteria.BuyItNowPriceMax = (double)val;
                        criteria.BuyItNowPriceMin = (double)val;
                        break;
                    case "BidCount":
                        criteria.BidCountMax = (int)val;
                        criteria.BidCountMin = (int)val;
                        break;
                    case "Condition":
                        criteria.Condition = (Condition)val;
                        break;
                    /*case "Country":
                        criteria.Country = (CountryCodeType)val;
                        break;*/
                    case "CurrentPrice":
                        criteria.PriceMax = (double)val;
                        criteria.PriceMin = (double)val;
                        break;
                    case "EndTime":
                        criteria.EndTimeMax = (DateTime)val;
                        criteria.EndTimeMin = (DateTime)val;
                        break;
                    /*case "ListingStatus":
                        criteria.ListingStatus = (ListingStatusCodeType)val;
                        break;*/
                    case "Title":
                    case "Description":
                        criteria.Keywords += (string)val + "|";
                        break;
                    /*case "Location":
                        criteria.Location = (string)val;
                        break;*/
                    case "StartTime":
                        criteria.StartTimeMin = (DateTime)val;
                        criteria.StartTimeMax = (DateTime)val;
                        break;
                    /*case "TimeLeft":
                        criteria.TimeLeft = (string)val;
                        break;*/
                    default:
                        throw new NotSupportedException();
                }
            }

            return expression;
        }

        private Expression VisitLess(BinaryExpression expression)
        {
            string mName;
            object val;

            if (expression.Left.NodeType == ExpressionType.MemberAccess || expression.Right.NodeType == ExpressionType.MemberAccess)
            {
                mName = ((MemberExpression)expression.Left).Member.Name;

                if (expression.Right.NodeType == ExpressionType.Constant)
                    val = ((ConstantExpression)expression.Right).Value;
                else
                    val = ((ConstantExpression)expression.Left).Value;

                /*if (expression.Right.NodeType == ExpressionType.Constant)
                    val = ((ConstantExpression)expression.Right).Value;
                else if (expression.Right.NodeType == ExpressionType.MemberAccess)
                    val = GetMemberValue((MemberExpression)expression.Right);
                else
                    throw new NotSupportedException("Expression type not supported: " + expression.Right.NodeType.ToString());
                */

                if (mName == "CurrentPrice")
                {
                    if (expression.NodeType == ExpressionType.LessThan)
                        criteria.PriceMax = ((double)val) - 1;
                    else if (expression.NodeType == ExpressionType.LessThanOrEqual)
                        criteria.PriceMax = (double)val;
                }
                else if (mName == "BuyItNowPrice")
                {
                    if (expression.NodeType == ExpressionType.LessThan)
                        criteria.BuyItNowPriceMax = ((double)val) - 1;
                    else if (expression.NodeType == ExpressionType.LessThanOrEqual)
                        criteria.BuyItNowPriceMax = (double)val;
                }
                else if (mName == "BidCount")
                {
                    if (expression.NodeType == ExpressionType.LessThan)
                        criteria.BidCountMax = ((int)val) - 1;
                    else if (expression.NodeType == ExpressionType.LessThanOrEqual)
                        criteria.BidCountMax = (int)val;
                }
                else if (mName == "StartTime")
                {
                    if (expression.NodeType == ExpressionType.LessThan)
                        criteria.StartTimeMax = (DateTime)val - new TimeSpan(1,0,0,0); // -1
                    else if (expression.NodeType == ExpressionType.LessThanOrEqual)
                        criteria.StartTimeMax = (DateTime)val;
                }
                else if (mName == "EndTime")
                {
                    if (expression.NodeType == ExpressionType.LessThan)
                        criteria.EndTimeMax = (DateTime)val - new TimeSpan(1,0,0,0); // -1
                    else if (expression.NodeType == ExpressionType.LessThanOrEqual)
                        criteria.EndTimeMax = (DateTime)val;
                }
                /*else if (mName == "TimeLeft")
                {
                    if (expression.NodeType == ExpressionType.LessThan)
                        criteria.TimeLeftMax = (string)val;
                    else if (expression.NodeType == ExpressionType.LessThanOrEqual)
                        criteria.TimeLeftMax = (string)val;
                }*/
                else
                    throw new NotSupportedException(mName);
            }

            return expression;
        }

        private Expression VisitGreater(BinaryExpression expression)
        {
            string mName;
            object val;

            if (expression.Left.NodeType == ExpressionType.MemberAccess || expression.Right.NodeType == ExpressionType.MemberAccess)
            {
                mName = ((MemberExpression)expression.Left).Member.Name;

                if (expression.Right.NodeType == ExpressionType.Constant)
                    val = ((ConstantExpression)expression.Right).Value;
                else
                    val = ((ConstantExpression)expression.Left).Value;

                /*if (expression.Right.NodeType == ExpressionType.Constant)
                    val = ((ConstantExpression)expression.Right).Value;
                else if (expression.Right.NodeType == ExpressionType.MemberAccess)
                    val = GetMemberValue((MemberExpression)expression.Right);
                else
                    throw new NotSupportedException("Expression type not supported: " + expression.Right.NodeType.ToString());
                */

                if (mName == "CurrentPrice")
                {
                    if (expression.NodeType == ExpressionType.GreaterThan)
                        criteria.PriceMin = ((double)val) + 1;
                    else if (expression.NodeType == ExpressionType.GreaterThanOrEqual)
                        criteria.PriceMin = (double)val;
                }
                else if (mName == "BuyItNowPrice")
                {
                    if (expression.NodeType == ExpressionType.GreaterThan)
                        criteria.BuyItNowPriceMin = ((double)val) + 1;
                    else if (expression.NodeType == ExpressionType.GreaterThanOrEqual)
                        criteria.BuyItNowPriceMin = (double)val;
                }
                else if (mName == "BidCount")
                {
                    if (expression.NodeType == ExpressionType.GreaterThan)
                        criteria.BidCountMin = ((int)val) + 1;
                    else if (expression.NodeType == ExpressionType.GreaterThanOrEqual)
                        criteria.BidCountMin = (int)val;
                }
                else if (mName == "StartTime")
                {
                    if (expression.NodeType == ExpressionType.GreaterThan)
                        criteria.StartTimeMin = (DateTime)val + new TimeSpan(1,0,0,0); // + 1;
                    else if (expression.NodeType == ExpressionType.GreaterThanOrEqual)
                        criteria.StartTimeMin = (DateTime)val;
                }
                else if (mName == "EndTime")
                {
                    if (expression.NodeType == ExpressionType.GreaterThan)
                        criteria.EndTimeMin = (DateTime)val + new TimeSpan(1,0,0,0); // + 1;
                    else if (expression.NodeType == ExpressionType.GreaterThanOrEqual)
                        criteria.EndTimeMin = (DateTime)val;
                }
                /*else if (mName == "TimeLeft")
                {
                    if (expression.NodeType == ExpressionType.GreaterThan)
                        criteria.TimeLeftMin = (string)val;
                    else if (expression.NodeType == ExpressionType.GreaterThanOrEqual)
                        criteria.TimeLeftMin = (string)val;
                }*/
                else
                    throw new NotSupportedException(mName);
            }

            return expression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            Expression arg;
            MemberExpression mExpr;
            string val;

            if ((expression.Method.DeclaringType == typeof(String)) && (expression.Method.Name == "Contains"))
            {
                if (expression.Object.NodeType == ExpressionType.MemberAccess)
                {
                    mExpr = (MemberExpression)expression.Object;

                    if (mExpr.Expression.Type == typeof(EbayItem))
                    {
                        arg = expression.Arguments[0];

                        if (arg.NodeType == ExpressionType.Constant)
                            val = (string)((ConstantExpression)arg).Value;
                        else if (arg.NodeType == ExpressionType.MemberAccess)
                            val = (string)GetMemberValue((MemberExpression)arg);
                        else
                            throw new NotSupportedException();

                        if (mExpr.Member.Name == "Description" || mExpr.Member.Name == "Title")
                            criteria.Keywords += val + "|";
                    }

                    return expression;
                }

            }
            /*else
            {
                throw new NotSupportedException(expression.Method.Name);
            }*/

            return base.VisitMethodCall(expression);
        }

        #region Helpers

        private Object GetMemberValue(MemberExpression memberExpression)
        {
          MemberInfo memberInfo;
          Object obj;

          if (memberExpression == null)
            throw new ArgumentNullException("memberExpression");

          // Get object
          if (memberExpression.Expression is ConstantExpression)
            obj = ((ConstantExpression)memberExpression.Expression).Value;
          else if (memberExpression.Expression is MemberExpression)
            obj = GetMemberValue((MemberExpression)memberExpression.Expression);
          else
            throw new NotSupportedException("Expression type not supported: " + memberExpression.Expression.GetType().FullName);

          // Get value
          memberInfo = memberExpression.Member;
          if (memberInfo is PropertyInfo)
          {
            PropertyInfo property = (PropertyInfo)memberInfo;
            return property.GetValue(obj, null);
          }
          else if (memberInfo is FieldInfo)
          {
            FieldInfo field = (FieldInfo)memberInfo;
            return field.GetValue(obj);
          }
          else
          {
            throw new NotSupportedException("MemberInfo type not supported: " + memberInfo.GetType().FullName);
          }
        }

        #endregion Helpers
    }
}
