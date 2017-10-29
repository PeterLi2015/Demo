using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using XDropsWater.CrossCutting;
using XDropsWater.CrossCutting.Reflection;

namespace XDropsWater.Dal.DataAccess.LamdaExpressions
{
    public class ExpressionUtil
    {
        /// <summary>
        /// 获取搜索表达式
        /// </summary>
        /// <returns></returns>
        public static Expression<Func<TEntity, bool>> GetSearchExpression<TEntity>(params IExpressionSearchItem<TEntity>[] searchItems)
        {
            if (ArrayUtil.IsEmptyArray(searchItems))
                return null;
            var param = GetEntityTypeParam(typeof(TEntity));
            var exp = GetSearchGroupExp(param, searchItems);
            return Expression.Lambda<Func<TEntity, bool>>(exp, param);
        }

        /// <summary>
        /// 获取单组搜索表达式
        /// </summary>
        /// <returns></returns>
        public static Expression GetSearchGroupExp<TEntity>(ParameterExpression param, params IExpressionSearchItem<TEntity>[] searchItems)
        {
            if (ArrayUtil.IsEmptyArray(searchItems)) return null;
            Expression exp = null;
            foreach (var item in searchItems)
            {
                Expression rightExp = item.GetExpression(param);
                if (exp == null)
                {
                    exp = rightExp;
                }
                else
                {
                    if (item.RelOperator == ExpLogicalRelOperator.And)
                    {
                        exp = Expression.And(exp, rightExp);
                    }
                    else if (item.RelOperator == ExpLogicalRelOperator.Or)
                    {
                        exp = Expression.Or(exp, rightExp);
                    }
                    else if (item.RelOperator == ExpLogicalRelOperator.Not)
                    {
                        //leftExp = Expression.Not(leftExp, exp);
                    }
                }
            }
            return exp;
        }

        /// <summary>
        /// 获取每个查询条件单元
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Expression GetSearchItemExpression(ExpSearchOperator op, string fieldName, string rightFieldName, object[] values, ParameterExpression param)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                return null;
            }
            if (values == null || values.Length == 0)
            {
                values = new object[] { null };
            }
            switch (op)
            {
                case ExpSearchOperator.Like:
                    return GetLikeExpression(param, fieldName, values[0].ToString());
                case ExpSearchOperator.Equal:
                    return GetEquelExpression(param, fieldName, rightFieldName, values[0]);
                case ExpSearchOperator.NotEqual:
                    return GetNotEquelExpression(param, fieldName, rightFieldName, values[0]);
                case ExpSearchOperator.GreaterThan:
                    return GetGreaterExpression(param, false, fieldName, rightFieldName, values[0]);
                case ExpSearchOperator.GreaterThanOrEqual:
                    return GetGreaterExpression(param, true, fieldName, rightFieldName, values[0]);
                case ExpSearchOperator.LessThan:
                    return GetLessExpression(param, false, fieldName, rightFieldName, values[0]);
                case ExpSearchOperator.LessThanOrEqual:
                    return GetLessExpression(param, true, fieldName, rightFieldName, values[0]);
                case ExpSearchOperator.In:
                    return GetInExpression(param, fieldName, values);
                case ExpSearchOperator.NotIn:
                    return GetNotInExpression(param, fieldName, values);
            }
            return null;
        }

        /// <summary>
        /// 获得实例数据属性的表达式 会自动嵌套获取
        /// </summary>
        /// <param name="entityTypeParam"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static MemberExpression GetEntityAttrMemberExp(ParameterExpression entityTypeParam, string fieldName, bool nullValue)
        {
            string[] fieldNames = fieldName.Split('.');
            MemberExpression body = null;
            for (int i = 0; i < fieldNames.Length; i++)
            {
                if (body == null)
                {
                    body = LambdaExpression.Property(entityTypeParam, fieldNames[i]);
                }
                else
                {
                    body = LambdaExpression.Property(body, fieldNames[i]);
                }
            }

            if (nullValue)
            {
                return body;
            }

            if (ReflectionUtil.IsNullableType(body.Type))
            {
                body = LambdaExpression.Property(body, "Value");
            }
            return body;
        }

        public static LambdaExpression GetEntityAttrLambdaExp(ParameterExpression entityTypeParam, string fieldName, bool nullValue)
        {
            return System.Linq.Expressions.Expression.Lambda(
                GetEntityAttrMemberExp(entityTypeParam, fieldName, false), entityTypeParam);
        }
        /// <summary>
        /// 获得数据类型的参数表达式
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static ParameterExpression GetEntityTypeParam(Type entityType)
        {
            return LambdaExpression.Parameter(entityType);
        }

        /// <summary>
        /// 根据表达式属性类型 获得 常量表达式
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static ConstantExpression GetConstExp(object value, Type valueType)
        {
            if (valueType == value.GetType())
            {
                return LambdaExpression.Constant(value, valueType);
            }
            try
            {
                //把常量转化成相对应的字段属性类型
                object newObj = Convert.ChangeType(value, valueType);
                return LambdaExpression.Constant(newObj, valueType);
            }
            catch
            {
                return LambdaExpression.Constant(value, valueType);
            }
        }
        /// <summary>
        /// 获得Like的表达式 
        /// </summary>
        private static Expression GetLikeExpression(ParameterExpression entityTypeParam, string fieldName, string value)
        {
            var body = GetEntityAttrMemberExp(entityTypeParam, fieldName, value == null);
            var constant = LambdaExpression.Constant(value, typeof(string));
            return Expression.Call(body, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), constant);
        }

        /// <summary>
        /// 获得Equel的表达式 
        /// </summary>
        private static Expression GetEquelExpression(ParameterExpression entityTypeParam, string fieldName, string rightFieldName, object value)
        {
            Expression left;
            Expression right;
            GetLeftRightExpressions(entityTypeParam, fieldName, rightFieldName, value, out left, out right);
            return Expression.Equal(left, right);
        }

        /// <summary>
        /// 获得NotEquel的表达式 
        /// </summary>
        private static Expression GetNotEquelExpression(ParameterExpression entityTypeParam, string fieldName, string rightFieldName, object value)
        {
            Expression left;
            Expression right;
            GetLeftRightExpressions(entityTypeParam, fieldName, rightFieldName, value, out left, out right);
            return Expression.NotEqual(left, right);
        }

        /// <summary>
        /// 获得Greater的表达式 containEquel 是否包含等于
        /// </summary>
        private static Expression GetGreaterExpression(
            ParameterExpression entityTypeParam, bool containEquel, string fieldName, string rightFieldName, object value)
        {
            Expression left;
            Expression right;
            GetLeftRightExpressions(entityTypeParam, fieldName, rightFieldName, value, out left, out right);
            if (containEquel)
            {
                return Expression.GreaterThanOrEqual(left, right);
            }
            return Expression.GreaterThan(left, right);
        }
        /// <summary>
        /// 获得Less的表达式 containEquel 是否包含等于
        /// </summary>
        private static Expression GetLessExpression(
            ParameterExpression entityTypeParam, bool containEquel, string fieldName, string rightFieldName, object value)
        {
            Expression left;
            Expression right;
            GetLeftRightExpressions(entityTypeParam, fieldName, rightFieldName, value, out left, out right);
            if (containEquel)
            {
                return Expression.LessThanOrEqual(left, right);
            }
            return Expression.LessThan(left, right);
        }

        private static void GetLeftRightExpressions(ParameterExpression entityTypeParam, string fieldName, string rightFieldName, object value, out Expression left, out Expression right)
        {
            //查询是否 join条件查询
            if (!string.IsNullOrEmpty(rightFieldName))
            {
                left = GetEntityAttrMemberExp(entityTypeParam, fieldName, false);
                right = GetEntityAttrMemberExp(entityTypeParam, rightFieldName, false);
            }
            else
            {
                left = GetEntityAttrMemberExp(entityTypeParam, fieldName, value == null);
                //排除空值情况
                if (value == null)
                {
                    right = LambdaExpression.Constant(null);
                }
                else
                {
                    right = GetConstExp(value, left.Type);
                }
            }
        }
        /// <summary>
        /// in的表达式
        /// </summary>
        /// <param name="entityTypeParam"></param>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private static Expression GetInExpression(ParameterExpression entityTypeParam, string fieldName, object[] values)
        {
            if (ArrayUtil.IsEmptyArray(values))
            {
                throw new Exception("IN表表达式条件值不能为空.");
            }
            var body = GetEntityAttrMemberExp(entityTypeParam, fieldName, false);
            var equals = values.Select(value => (Expression)Expression.Equal(body, Expression.Constant(value, body.Type)));
            return equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
        }
        /// <summary>
        /// NotIn的表达式
        /// </summary>
        /// <param name="entityTypeParam"></param>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private static Expression GetNotInExpression(ParameterExpression entityTypeParam, string fieldName, object[] values)
        {
            if (ArrayUtil.IsEmptyArray(values))
            {
                throw new Exception("NotIn表表达式条件值不能为空.");
            }
            var body = GetEntityAttrMemberExp(entityTypeParam, fieldName, false);
            var equals = values.Select(value => (Expression)Expression.NotEqual(body, Expression.Constant(value, body.Type)));
            return equals.Aggregate<Expression>((accumulate, equal) => Expression.And(accumulate, equal));
        }
        /// <summary>
        /// 根据属性表达式获取属性名字
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public string GetNameByPropertyExp<TEntity>(Expression<Func<TEntity, object>> property)
            where TEntity : class
        {
            MemberExpression memberExp = null;

            memberExp = property.Body as MemberExpression;
            if (memberExp == null)
            {
                //判断是否为可控操作符
                var unaryExp = property.Body as UnaryExpression;
                if (unaryExp != null)
                {
                    memberExp = unaryExp.Operand as MemberExpression;
                }
            }
            if (memberExp == null)
            {

                throw new FormatException("propertys必须为属性表达式.");
            }
            return memberExp.Member.Name;
        }
        /// <summary>
        /// 获取排序字段表达式
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="orderCond"></param>
        /// <returns></returns>
        public static LambdaExpression GetOrderSortExpression(Type entityType, string orderCond)
        {
            var parameter = Expression.Parameter(entityType);
            Expression propertyAccess = GetEntityAttrMemberExp(parameter, orderCond, false);
            return Expression.Lambda(propertyAccess, parameter);
        }
        /// <summary>
        /// 获取公式内容
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetExpressionBody<TEntity>(Expression<Func<TEntity, object>> expression)
        {
            if (expression == null)
                return string.Empty;
            var result = expression.Body.ToString();
            int index = result.IndexOf(".");
            if (index != -1 && index <= result.Length - 2)
                return result.Substring(index + 1).TrimEnd(')');
            return string.Empty;
        }
    }

    public abstract class IExpressionSearchItem<TEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        public IExpressionSearchItem(ExpLogicalRelOperator ro)
        {
            this.RelOperator = ro;
        }

        /// <summary>
        /// 
        /// </summary>
        public IExpressionSearchItem()
            : this(ExpLogicalRelOperator.And)
        {
        }

        /// <summary>
        /// 逻辑关系符
        /// </summary>
        public ExpLogicalRelOperator RelOperator { get; set; }
        /// <summary>
        /// child SearchItems
        /// </summary>
        private IList<IExpressionSearchItem<TEntity>> ChildSearchItems = new List<IExpressionSearchItem<TEntity>>();

        private bool HasChild
        {
            get
            {
                return ChildSearchItems.Any();
            }
        }

        public IExpressionSearchItem<TEntity> AddChild(IExpressionSearchItem<TEntity> child)
        {
            ChildSearchItems.Add(child);
            return this;
        }

        public Expression GetExpression(ParameterExpression param)
        {
            if (this.HasChild)
                return ExpressionUtil.GetSearchGroupExp(param, this.ChildSearchItems.ToArray());
            return GetMyExpression(param);
        }

        protected abstract Expression GetMyExpression(ParameterExpression param);
    }

    /// <summary>
    /// 查询条件单元
    /// </summary>
    public class ConditionSearchItem<TEntity> : IExpressionSearchItem<TEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        public ConditionSearchItem(Expression<Func<TEntity, bool>> condition, ExpLogicalRelOperator ro)
            : base(ro)
        {
            Init(condition);
        }

        /// <summary>
        /// 
        /// </summary>
        public ConditionSearchItem(Expression<Func<TEntity, bool>> condition)
            : base()
        {
            Init(condition);
        }

        private void Init(Expression<Func<TEntity, bool>> condition)
        {
            this.Condition = condition;
        }

        /// <summary>
        /// 查询字段名称
        /// </summary>
        public Expression<Func<TEntity, bool>> Condition { get; set; }

        protected override Expression GetMyExpression(ParameterExpression param)
        {
            ParameterExpressionVisitor visitor = new ParameterExpressionVisitor();
            visitor.Parameter = param;
            Expression<Func<TEntity, bool>> res = this.Condition;
            return visitor.Modify(res.Body);
        }
    }

    /// <summary>
    /// 查询条件单元
    /// </summary>
    public class ExpressionSearchItem<TEntity> : IExpressionSearchItem<TEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        public ExpressionSearchItem()
            : base()
        {
            this.Operator = ExpSearchOperator.Equal;
        }

        /// <summary>
        /// 查询字段名称
        /// </summary>
        public Expression<Func<TEntity, object>> FieldName { get; set; }
        /// <summary>
        /// 右查询字段名称
        /// </summary>
        public Expression<Func<TEntity, object>> RightFieldName { get; set; }
        /// <summary>
        /// 搜索操作数
        /// </summary>
        public object[] Values { get; set; }
        /// <summary>
        /// 搜索操作符
        /// </summary>
        public ExpSearchOperator Operator { get; set; }

        protected override Expression GetMyExpression(ParameterExpression param)
        {
            return ExpressionUtil.GetSearchItemExpression(this.Operator, ExpressionUtil.GetExpressionBody(this.FieldName), ExpressionUtil.GetExpressionBody(this.RightFieldName), this.Values, param);
        }
    }

    public class ParameterExpressionVisitor : ExpressionVisitor
    {
        public ParameterExpression Parameter { get; set; }

        public Expression Modify(Expression exp)
        {
            return this.Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            return Parameter;
        }
    }

    /// <summary>
    /// 操作符枚举
    /// </summary>
    public enum ExpSearchOperator
    {
        /// <summary>
        /// 小于
        /// </summary>
        LessThan = 1,
        /// <summary>
        /// 小于等于
        /// </summary>
        LessThanOrEqual = 2,
        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan = 3,
        /// <summary>
        /// 大于等于
        /// </summary>
        GreaterThanOrEqual = 4,
        /// <summary>
        /// 等于
        /// </summary>
        Equal = 5,
        /// <summary>
        /// sql的like语法
        /// </summary>
        Like = 6,
        /// <summary>
        /// sql的in语法
        /// </summary>
        In = 7,
        /// <summary>
        /// sql的not in语法
        /// </summary>
        NotIn = 8,
        /// <summary>
        /// 不等于
        /// </summary>
        NotEqual = 9
    }

    /// <summary>
    /// 逻辑关系枚举
    /// </summary>
    public enum ExpLogicalRelOperator
    {
        /// <summary>
        /// 且
        /// </summary>
        And = 1,
        /// <summary>
        /// 或
        /// </summary>
        Or = 2,
        /// <summary>
        /// 非
        /// </summary>
        Not = 3
    }
}
