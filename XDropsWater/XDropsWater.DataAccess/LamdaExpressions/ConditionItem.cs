using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using XDropsWater.Dal.DataAccess.LamdaExpressions;

namespace XDropsWater.DataAccess.LamdaExpressions
{
    public class ConditionItem<TEntity> where TEntity : class
    {
        private List<IExpressionSearchItem<TEntity>> searchItemList = new List<IExpressionSearchItem<TEntity>>();
        private bool returnNull = false;

        public ConditionItem()
        {
        }

        public ConditionItem<TEntity> Join
        {
            get
            {
                return new ConditionItem<TEntity>();
            }
        }

        public ConditionItem<TEntity> And(ExpSearchItem<TEntity> condition, Func<bool> notAppend = null)
        {
            if (notAppend != null && notAppend())
                return AddEmpty(ExpLogicalRelOperator.And);
            return AddSearchItem(condition.TranToSearchItem(ExpLogicalRelOperator.And));
        }

        public ConditionItem<TEntity> And(Expression<Func<TEntity, bool>> condition, Func<bool> notAppend = null)
        {
            if (notAppend != null && notAppend())
                return AddEmpty(ExpLogicalRelOperator.And);
            return AddSearchItem(condition, ExpLogicalRelOperator.And);
        }

        public ConditionItem<TEntity> And(ConditionItem<TEntity> cb, Func<bool> notAppend = null)
        {
            return AddSearchItem(cb, notAppend, ExpLogicalRelOperator.And);
        }

        public ConditionItem<TEntity> Or(Expression<Func<TEntity, bool>> condition, Func<bool> notAppend = null)
        {
            if (notAppend != null && notAppend())
                return AddEmpty(ExpLogicalRelOperator.Or);
            return AddSearchItem(condition, ExpLogicalRelOperator.Or);
        }

        public ConditionItem<TEntity> Or(ExpSearchItem<TEntity> condition, Func<bool> notAppend = null)
        {
            if (notAppend != null && notAppend())
                return AddEmpty(ExpLogicalRelOperator.Or);
            return AddSearchItem(condition.TranToSearchItem(ExpLogicalRelOperator.Or));
        }

        public ConditionItem<TEntity> Or(ConditionItem<TEntity> cb, Func<bool> notAppend = null)
        {
            return AddSearchItem(cb, notAppend, ExpLogicalRelOperator.Or);
        }

        public ConditionItem<TEntity> Limit(Func<bool> limit)
        {
            if (limit != null && limit())
                returnNull = true;
            return this;
        }

        public Expression<Func<TEntity, bool>> Condition()
        {
            if (returnNull)
                return e => false;
            return ExpressionUtil.GetSearchExpression(searchItemList.ToArray());
        }

        private ConditionItem<TEntity> AddEmpty(ExpLogicalRelOperator ro)
        {
            if (ro == ExpLogicalRelOperator.And)
                return AndTrue();
            return OrFalse();
        }

        private ConditionItem<TEntity> AndTrue()
        {
            return AddSearchItem(e => true, ExpLogicalRelOperator.And);
        }

        private ConditionItem<TEntity> OrFalse()
        {
            return AddSearchItem(e => false, ExpLogicalRelOperator.Or);
        }

        private ConditionItem<TEntity> AddSearchItem(IExpressionSearchItem<TEntity> esi)
        {
            searchItemList.Add(esi);
            return this;
        }

        private ConditionItem<TEntity> AddSearchItem(ConditionItem<TEntity> cb, Func<bool> notAppend, ExpLogicalRelOperator ro)
        {
            if (notAppend != null && notAppend())
                return AddEmpty(ro);
            if (cb.searchItemList.Count == 0)
                return this;
            var esi = new ExpressionSearchItem<TEntity>() { RelOperator = ro };
            cb.searchItemList.ForEach((item) =>
            {
                esi.AddChild(item);
            });
            return AddSearchItem(esi);
        }

        private ConditionItem<TEntity> AddSearchItem(Expression<Func<TEntity, bool>> condition, ExpLogicalRelOperator ro)
        {
            return AddSearchItem(new ConditionSearchItem<TEntity>(condition, ro));
        }

        public ExpSearchItem<TEntity> LessThan(Expression<Func<TEntity, object>> fieldName, Func<object> getVal)
        {
            return ExpCreator.Create(fieldName, ExpSearchOperator.LessThan, getVal);
        }

        public ExpSearchItem<TEntity> Equal(Expression<Func<TEntity, object>> fieldName, Func<object> getVal)
        {
            return ExpCreator.Create(fieldName, ExpSearchOperator.Equal, getVal);
        }

        public ExpSearchItem<TEntity> GreaterThan(Expression<Func<TEntity, object>> fieldName, Func<object> getVal)
        {
            return ExpCreator.Create(fieldName, ExpSearchOperator.GreaterThan, getVal);
        }

        public ExpSearchItem<TEntity> GreaterThanOrEqual(Expression<Func<TEntity, object>> fieldName, Func<object> getVal)
        {
            return ExpCreator.Create(fieldName, ExpSearchOperator.GreaterThanOrEqual, getVal);
        }

        public ExpSearchItem<TEntity> LessThanOrEqual(Expression<Func<TEntity, object>> fieldName, Func<object> getVal)
        {
            return ExpCreator.Create(fieldName, ExpSearchOperator.LessThanOrEqual, getVal);
        }

        public ExpSearchItem<TEntity> Like(Expression<Func<TEntity, object>> fieldName, Func<object> getVal)
        {
            return ExpCreator.Create(fieldName, ExpSearchOperator.Like, getVal);
        }

        public ExpSearchItem<TEntity> NotEqual(Expression<Func<TEntity, object>> fieldName, Func<object> getVal)
        {
            return ExpCreator.Create(fieldName, ExpSearchOperator.NotEqual, getVal);
        }

        public ExpSearchItem<TEntity> In(Expression<Func<TEntity, object>> fieldName, Func<object[]> getVals)
        {
            return ExpCreator.Create(fieldName, ExpSearchOperator.In, getVals);
        }

        public ExpSearchItem<TEntity> NotIn(Expression<Func<TEntity, object>> fieldName, Func<object[]> getVals)
        {
            return ExpCreator.Create(fieldName, ExpSearchOperator.NotIn, getVals);
        }

        public ExpSearchItem<TEntity> LessThan(Expression<Func<TEntity, object>> fieldName, Expression<Func<TEntity, object>> rightFieldName)
        {
            return ExpCreator.Create(fieldName, ExpSearchOperator.LessThan, rightFieldName);
        }

        public ExpSearchItem<TEntity> Equal(Expression<Func<TEntity, object>> fieldName, Expression<Func<TEntity, object>> rightFieldName)
        {
            return ExpCreator.Create(fieldName, ExpSearchOperator.Equal, rightFieldName);
        }

        public ExpSearchItem<TEntity> GreaterThan(Expression<Func<TEntity, object>> fieldName, Expression<Func<TEntity, object>> rightFieldName)
        {
            return ExpCreator.Create(fieldName, ExpSearchOperator.GreaterThan, rightFieldName);
        }

        public ExpSearchItem<TEntity> GreaterThanOrEqual(Expression<Func<TEntity, object>> fieldName, Expression<Func<TEntity, object>> rightFieldName)
        {
            return ExpCreator.Create(fieldName, ExpSearchOperator.GreaterThanOrEqual, rightFieldName);
        }

        public ExpSearchItem<TEntity> LessThanOrEqual(Expression<Func<TEntity, object>> fieldName, Expression<Func<TEntity, object>> rightFieldName)
        {
            return ExpCreator.Create(fieldName, ExpSearchOperator.LessThanOrEqual, rightFieldName);
        }

        public ExpSearchItem<TEntity> NotEqual(Expression<Func<TEntity, object>> fieldName, Expression<Func<TEntity, object>> rightFieldName)
        {
            return ExpCreator.Create(fieldName, ExpSearchOperator.NotEqual, rightFieldName);
        }
    }

    public class ExpCreator
    {
        public static ExpSearchItem<T> Create<T>(Expression<Func<T, object>> fieldName, ExpSearchOperator op, Func<object> getVal) where T : class
        {
            return new ExpSearchItem<T>(fieldName, getVal) { Operator = op };
        }

        public static ExpSearchItem<T> Create<T>(Expression<Func<T, object>> fieldName, ExpSearchOperator op, Func<object[]> getVals) where T : class
        {
            return new ExpSearchItem<T>(fieldName, getVals) { Operator = op };
        }

        public static ExpSearchItem<T> Create<T>(Expression<Func<T, object>> fieldName, ExpSearchOperator op, Expression<Func<T, object>> rightFieldName) where T : class
        {
            return new ExpSearchItem<T>(fieldName, rightFieldName) { Operator = op };
        }
    }

    public class ExpSearchItem<TEntity> where TEntity : class
    {
        internal ExpSearchItem()
        { }

        internal ExpSearchItem(Expression<Func<TEntity, object>> fieldName, Func<object> getVal)
        {
            FieldName = fieldName;
            GetVals = () => new object[] { getVal() };
        }

        internal ExpSearchItem(Expression<Func<TEntity, object>> fieldName, Func<object[]> getVals)
        {
            FieldName = fieldName;
            GetVals = getVals;
        }

        internal ExpSearchItem(Expression<Func<TEntity, object>> fieldName, Expression<Func<TEntity, object>> rightFieldName)
        {
            FieldName = fieldName;
            RightFieldName = rightFieldName;
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
        /// 延迟计算，由于某些值的计算取决于先决条件
        /// </summary>
        Func<object[]> GetVals { get; set; }
        /// <summary>
        /// 搜索操作符
        /// </summary>
        public ExpSearchOperator Operator { get; set; }

        public ExpressionSearchItem<TEntity> TranToSearchItem(ExpLogicalRelOperator ro)
        {
            return new ExpressionSearchItem<TEntity>()
            {
                FieldName = this.FieldName,
                RightFieldName = this.RightFieldName,
                Operator = this.Operator,
                RelOperator = ro,
                Values = this.GetVals()
            };
        }
    }
}
