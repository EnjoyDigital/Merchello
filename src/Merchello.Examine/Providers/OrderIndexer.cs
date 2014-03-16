﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Examine;
using Examine.LuceneEngine;
using Examine.LuceneEngine.Config;
using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Examine.Providers
{
    public class OrderIndexer : BaseMerchelloIndexer
    {
        protected override void PerformIndexAll(string type)
        {
            if (!SupportedTypes.Contains(type)) return;

            var invoices = DataService.InvoiceDataService.GetAll();
            var invoicesArray = invoices as IInvoice[] ?? invoices.ToArray();

            if (!invoicesArray.Any()) return;
            var nodes = invoicesArray.Select(i => i.SerializeToXml().Root).ToList();

            AddNodesToIndex(nodes, IndexTypes.Invoice);
        }

        public override void RebuildIndex()
        {
            DataService.LogService.AddVerboseLog(-1, "Rebuilding index");

            EnsureIndex(true);

            PerformIndexAll(IndexTypes.Order);
            //base.RebuildIndex();
        }

        /// <summary>
        /// Adds the order to the index
        /// </summary>
        /// <param name="order"></param>
        /// <remarks>For testing</remarks>
        internal void AddOrderToIndex(IOrder order)
        {
            var nodes = new List<XElement> {order.SerializeToXml().Root};
            AddNodesToIndex(nodes, IndexTypes.Order);
        }

        /// <summary>
        /// Removes the order from the index
        /// </summary>
        /// <param name="order"></param>
        /// <remarks>For testing</remarks>
        internal void DeleteOrderFromIndex(IOrder order)
        {            
            DeleteFromIndex(((Order)order).ExamineId.ToString(CultureInfo.InvariantCulture));
        }



        protected override IEnumerable<string> SupportedTypes
        {
            get { return new[] { IndexTypes.Order }; }
        }

        internal static readonly List<StaticField> IndexFieldPolicies
            = new List<StaticField>()
            {
                new StaticField("orderKey", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("invoiceKey", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("orderNumberPrefix", FieldIndexTypes.NOT_ANALYZED, true, string.Empty),
                new StaticField("orderNumber", FieldIndexTypes.ANALYZED, true, string.Empty),
                new StaticField("prefixedOrderNumber", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("orderDate", FieldIndexTypes.ANALYZED, true, "DATETIME"),
                new StaticField("orderStatusKey", FieldIndexTypes.ANALYZED, false, string.Empty),
                new StaticField("versionKey", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("exported", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("total", FieldIndexTypes.ANALYZED, true, "DOUBLE"),
                new StaticField("orderStatus", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("orderItems", FieldIndexTypes.NOT_ANALYZED, false, string.Empty),
                new StaticField("createDate", FieldIndexTypes.NOT_ANALYZED, false, "DATETIME"),
                new StaticField("updateDate", FieldIndexTypes.NOT_ANALYZED, false, "DATETIME"),
                new StaticField("allDocs", FieldIndexTypes.ANALYZED, false, string.Empty)
            };


        /// <summary>
        /// Creates an IIndexCriteria object based on the indexSet passed in and our DataService
        /// </summary>
        /// <param name="indexSet"></param>
        /// <returns></returns>
        /// <remarks>
        /// If we cannot initialize we will pass back empty indexer data since we cannot read from the database
        /// </remarks>
        protected override IIndexCriteria GetIndexerData(IndexSet indexSet)
        {
            return indexSet.ToIndexCriteria(DataService.OrderDataService.GetIndexFieldNames(), IndexFieldPolicies);
        }

        /// <summary>
        /// return the index policy for the field name passed in, if not found, return normal
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        protected override FieldIndexTypes GetPolicy(string fieldName)
        {
            var def = IndexFieldPolicies.Where(x => x.Name == fieldName).ToArray();
            return (def.Any() == false ? FieldIndexTypes.ANALYZED : def.Single().IndexType);
        }

    }
}