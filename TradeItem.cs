using System;
using com.tc.frameworks.ictseos.eoaccount;
using com.tc.frameworks.ictseos.eocmdtymkt;
using com.tc.frameworks.ictseos.eoposition;
using com.tc.frameworks.ictseos.eoutility;
using cs.fw.eo.Entity;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Collections.Generic;
using com.tc.frameworks.ictseos.eocommaccount;
using System.Xml.Serialization;
using cs.fw.eo.fetchspec;
using cs.fw.eo;
using System.Xml.Schema;
using System.Xml;
using System.IO;
using com.tc.frameworks.ictseos.parcelshipment;
using com.tc.frameworks.ictseos.eocredit;
using com.tc.frameworks.ictseos.eoallocation;
using System.Collections;

namespace com.tc.frameworks.ictseos.eotrade
{
    [Serializable]
    [XmlInclude(typeof(WetPhysicalTradeItem))]
    [XmlInclude(typeof(DryPhysicalTradeItem))]
    [XmlInclude(typeof(StorageTradeItem))]
    [XmlInclude(typeof(TransportTradeItem))]
    [XmlInclude(typeof(BunkerTradeItem))]
    //[XmlType(Namespace = "http://tradecapture/entity")]
    public partial class TradeItem : EntityRecord, ISerializable, IXmlSerializable, IComparable
    {
        public TradeItem()
            : base()
        {
            Initialize();
        }

        public TradeItem(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Initialize();
        }
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public int? beginInit
        {
            set
            {
                if (value == 1)
                    BeginInit();
            }
        }
        public int? tradeNum
        {
            get { return valueForKey<int?>("tradeNum"); }
            set { takeValueForKey("tradeNum", value); }
        }

        public int? orderNum
        {
            get { return valueForKey<int?>("orderNum"); }
            set { takeValueForKey("orderNum", value); }
        }

        public int? itemNum
        {
            get { return valueForKey<int?>("itemNum"); }
            set { takeValueForKey("itemNum", value); }
        }

        public TradeOrder tradeOrder
        {
            get { return valueForKey<TradeOrder>("tradeOrder"); }
            set { 
                if (this.valueForKey<TradeOrder>("tradeOrder") != null)
                    this.valueForKey<TradeOrder>("tradeOrder").WeakPropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(TradeOrderPropertyChanged);

                takeValueForKey("tradeOrder", value);

                if (this.valueForKey<TradeOrder>("tradeOrder") != null)
                    this.valueForKey<TradeOrder>("tradeOrder").WeakPropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(TradeOrderPropertyChanged);
            }
        }

        public string cmdtyCode
        {
            get { return valueForKey<string>("cmdtyCode"); }
            set { 
                takeValueForKey("cmdtyCode", value);
                //setRiskTradingPeriod();
                if (this is WetPhysicalTradeItem)
                        (this as WetPhysicalTradeItem).autopoolCriteria=null;
            }
        }

        public string itemStatusCode
        {
            get { return valueForKey<string>("itemStatusCode"); }
            set { takeValueForKey("itemStatusCode", value); }
        }

        public string itemType
        {
            get { return valueForKey<string>("itemType"); }
            set { takeValueForKey("itemType", value); }
        }

        public string pSInd
        {
            get { return valueForKey<string>("pSInd"); }
            set { takeValueForKey("pSInd", value); }
        }

        public decimal? avgPrice
        {
            get { return valueForKey<decimal?>("avgPrice"); }
            set { takeValueForKey("avgPrice", value); }
        }

        public int? bookingCompNum
        {
            get { return valueForKey<int?>("bookingCompNum"); }
            set { 
                takeValueForKey("bookingCompNum", value);
                if (this is WetPhysicalTradeItem)
                    (this as WetPhysicalTradeItem).autopoolCriteria = null;
            }
        }

        public string calendarCode
        {
            get { return valueForKey<string>("calendarCode"); }
            set { takeValueForKey("calendarCode", value); }
        }

        public decimal? contrQty
        {
            get { return valueForKey<decimal?>("contrQty"); }
            set { takeValueForKey("contrQty", value); }
        }

        public string contrQtyPeriodicity
        {
            get { return valueForKey<string>("contrQtyPeriodicity"); }
            set { takeValueForKey("contrQtyPeriodicity", value); }
        }

        public string contrQtyUomCode
        {
            get { return valueForKey<string>("contrQtyUomCode"); }
            set { takeValueForKey("contrQtyUomCode", value); }
        }

        public string committedQtyUomCode
        {
            get { return valueForKey<string>("committedQtyUomCode"); }
            set { takeValueForKey("committedQtyUomCode", value); }
        }

        public string estimateInd
        {
            get { return valueForKey<string>("estimateInd"); }
            set { takeValueForKey("estimateInd", value); }
        }

        public string formulaInd
        {
            get { return valueForKey<string>("formulaInd"); }
            set { takeValueForKey("formulaInd", value); }
        }

        public string isLcAssigned
        {
            get { return valueForKey<string>("isLcAssigned"); }
            set { takeValueForKey("isLcAssigned", value); }
        }

        public string isRcAssigned
        {
            get { return valueForKey<string>("isRcAssigned"); }
            set { takeValueForKey("isRcAssigned", value); }
        }

        public string gtcCode
        {
            get { return valueForKey<string>("gtcCode"); }
            set { takeValueForKey("gtcCode", value); }
        }

        public decimal? openQty
        {
            get { return valueForKey<decimal?>("openQty"); }
            set { takeValueForKey("openQty", value); }
        }
        //[I#MERCO-6195] added by Kiran Kumar Poloju on 8th Dec 2014.
        public string moveOpenQtyInd
        {
            get { return valueForKey<string>("moveOpenQtyInd"); }
            set { takeValueForKey("moveOpenQtyInd", value); }
        }
        public string openQtyUomCode
        {
            get { return valueForKey<string>("openQtyUomCode"); }
            set { takeValueForKey("openQtyUomCode", value); }
        }

        public string priceCurrCode
        {
            get { return valueForKey<string>("priceCurrCode"); }
            set { takeValueForKey("priceCurrCode", value); }
        }

        public string priceUomCode
        {
            get { return valueForKey<string>("priceUomCode"); }
            set { takeValueForKey("priceUomCode", value); }
        }

        public string pricedQtyUomCode
        {
            get { return valueForKey<string>("pricedQtyUomCode"); }
            set { takeValueForKey("pricedQtyUomCode", value); }
        }

        public string b2bTradeItem
        {
            get { return valueForKey<string>("b2bTradeItem"); }
            set { takeValueForKey("b2bTradeItem", value); }
        }
        public string sapOrderNum
        {
            get { return valueForKey<string>("sapOrderNum"); }
            set { takeValueForKey("sapOrderNum", value); }
        }
        public int? realPortNum
        {
            get { return valueForKey<int?>("realPortNum"); }
            set { takeValueForKey("realPortNum", value); }
        }
		public string riskMktCode
        {
            get { return valueForKey<string>("riskMktCode"); }
            set { 
                takeValueForKey("riskMktCode", value);
                //single line of code commented by PAdma RAo on 28 Jul 2014 based on issue 1398402. This line has ben moved to the method "getDetailDataViaIBatis"
                //setRiskTradingPeriod();
                if (this is WetPhysicalTradeItem)
                    (this as WetPhysicalTradeItem).autopoolCriteria = null;
            }
        }

        public string schQtyUomCode
        {
            get { return valueForKey<string>("schQtyUomCode"); }
            set { takeValueForKey("schQtyUomCode", value); }
        }

        public int? schedStatus
        {
            get { return valueForKey<int?>("schedStatus"); }
            set { takeValueForKey("schedStatus", value); }
        }

        public string titleMktCode
        {
            get { return valueForKey<string>("titleMktCode"); }
            set { takeValueForKey("titleMktCode", value); }
        }

        public decimal? totalPricedQty
        {
            get { return valueForKey<decimal?>("totalPricedQty"); }
            set { takeValueForKey("totalPricedQty", value); }
        }

        public decimal? totalSchQty
        {
            get { return valueForKey<decimal?>("totalSchQty"); }
            set { takeValueForKey("totalSchQty", value); }
        }

        public int? transId
        {
            get { return valueForKey<int?>("transId"); }
            set { takeValueForKey("transId", value); }
        }

        public string dischPortLocCode
        {
            get { return valueForKey<string>("dischPortLocCode"); }
            set { takeValueForKey("dischPortLocCode", value); }
        }

        public string loadPortLocCode
        {
            get { return valueForKey<string>("loadPortLocCode"); }
            set { takeValueForKey("loadPortLocCode", value); }
        }

        public Location loadPortLocation
        {
            get { return valueForKey<Location>("loadPortLocation"); }
            set { takeValueForKey("loadPortLocation", value); }
        }

        public Location distPortLocation
        {
            get { return valueForKey<Location>("distPortLocation"); }
            set { takeValueForKey("distPortLocation", value); }
        }

        public string accumPeriodicity
        {
            get { return valueForKey<string>("accumPeriodicity"); }
            set { takeValueForKey("accumPeriodicity", value); }
        }

        public DateTime? amendCreationDate
        {
            get { return valueForKey<DateTime?>("amendCreationDate"); }
            set { takeValueForKey("amendCreationDate", value); }
        }

        public DateTime? amendEffectEndDate
        {
            get { return valueForKey<DateTime?>("amendEffectEndDate"); }
            set { takeValueForKey("amendEffectEndDate", value); }
        }

        public DateTime? amendEffectStartDate
        {
            get { return valueForKey<DateTime?>("amendEffectStartDate"); }
            set { takeValueForKey("amendEffectStartDate", value); }
        }

        public int? amendNum
        {
            get { return valueForKey<int?>("amendNum"); }
            set { takeValueForKey("amendNum", value); }
        }

        public string billingType
        {
            get { return valueForKey<string>("billingType"); }
            set { takeValueForKey("billingType", value); }
        }

        public decimal? brkrCommAmt
        {
            get { return valueForKey<decimal?>("brkrCommAmt"); }
            set { takeValueForKey("brkrCommAmt", value); }
        }

        public string brkrCommCurrCode
        {
            get { return valueForKey<string>("brkrCommCurrCode"); }
            set { takeValueForKey("brkrCommCurrCode", value); }
        }

        public string brkrCommUomCode
        {
            get { return valueForKey<string>("brkrCommUomCode"); }
            set { takeValueForKey("brkrCommUomCode", value); }
        }

        public int? brkrContNum
        {
            get { return valueForKey<int?>("brkrContNum"); }
            set { takeValueForKey("brkrContNum", value); }
        }

        public int? brkrNum
        {
            get { return valueForKey<int?>("brkrNum"); }
            set { takeValueForKey("brkrNum", value); }
        }

        public string brkrRefNum
        {
            get { return valueForKey<string>("brkrRefNum"); }
            set { takeValueForKey("brkrRefNum", value); }
        }

        public int? cmntNum
        {
            get { return valueForKey<int?>("cmntNum"); }
            set { takeValueForKey("cmntNum", value); }
        }

        public string futTraderInit
        {
            get { return valueForKey<string>("futTraderInit"); }
            set { takeValueForKey("futTraderInit", value); }
        }

        public string hedgeCurrCode
        {
            get { return valueForKey<string>("hedgeCurrCode"); }
            set { takeValueForKey("hedgeCurrCode", value); }
        }

        public string hedgeMultiDivInd
        {
            get { return valueForKey<string>("hedgeMultiDivInd"); }
            set { takeValueForKey("hedgeMultiDivInd", value); }
        }

        public string hedgePosInd
        {
            get { return valueForKey<string>("hedgePosInd"); }
            set { takeValueForKey("hedgePosInd", value); }
        }

        public decimal? hedgeRate
        {
            get { return valueForKey<decimal?>("hedgeRate"); }
            set { takeValueForKey("hedgeRate", value); }
        }

        public string idmsAcctAlloc
        {
            get { return valueForKey<string>("idmsAcctAlloc"); }
            set { takeValueForKey("idmsAcctAlloc", value); }
        }

        public string idmsBbRefNum
        {
            get { return valueForKey<string>("idmsBbRefNum"); }
            set { takeValueForKey("idmsBbRefNum", value); }
        }

        public string idmsContrNum
        {
            get { return valueForKey<string>("idmsContrNum"); }
            set { takeValueForKey("idmsContrNum", value); }
        }

        public string idmsProfitCenter
        {
            get { return valueForKey<string>("idmsProfitCenter"); }
            set { takeValueForKey("idmsProfitCenter", value); }
        }

        public int? parentItemNum
        {
            get { return valueForKey<int?>("parentItemNum"); }
            set { takeValueForKey("parentItemNum", value); }
        }

        public string poolingPortInd
        {
            get { return valueForKey<string>("poolingPortInd"); }
            set {
                takeValueForKey("poolingPortInd", value);
                if (this is WetPhysicalTradeItem)
                    (this as WetPhysicalTradeItem).autopoolCriteria = null;
            }
        }

        public int? poolingPortNum
        {
            get { return valueForKey<int?>("poolingPortNum"); }
            set { takeValueForKey("poolingPortNum", value); }
        }

        public string poolingType
        {
            get { return valueForKey<string>("poolingType"); }
            set { takeValueForKey("poolingType", value); }
        }

        public int? recapItemNum
        {
            get { return valueForKey<int?>("recapItemNum"); }
            set { takeValueForKey("recapItemNum", value); }
        }

        public string stripItemStatus
        {
            get { return valueForKey<string>("stripItemStatus"); }
            set { takeValueForKey("stripItemStatus", value); }
        }

        public int? summaryItemNum
        {
            get { return valueForKey<int?>("summaryItemNum"); }
            set { takeValueForKey("summaryItemNum", value); }
        }

        public string tradingPrd
        {
            get { return valueForKey<string>("tradingPrd"); }
            set
            {
                takeValueForKey("tradingPrd", value);
                //setRiskTradingPeriod();
                if (this is WetPhysicalTradeItem)
                    (this as WetPhysicalTradeItem).autopoolCriteria = null;
            }
        }

        public decimal? uomConvRate
        {
            get { return valueForKey<decimal?>("uomConvRate"); }
            set { takeValueForKey("uomConvRate", value); }
        }

        public int? maxAccumNum
        {
            get { return valueForKey<int?>("maxAccumNum"); }
            set { takeValueForKey("maxAccumNum", value); }
        }

        public DateTime? formulaDeclarDate
        {
            get { return valueForKey<DateTime?>("formulaDeclarDate"); }
            set { takeValueForKey("formulaDeclarDate", value); }
        }

        public string purchasingGroup
        {
            get { return valueForKey<string>("purchasingGroup"); }
            set { takeValueForKey("purchasingGroup", value); }
        }

        public string originCountryCode
        {
            get { return valueForKey<string>("originCountryCode"); }
            set { takeValueForKey("originCountryCode", value); }
        }

        public string excpAddnsCode
        {
            get { return valueForKey<string>("excpAddnsCode"); }
            set { takeValueForKey("excpAddnsCode", value); }
        }
        public ExceptionsAdditions exceptionsAdditions
        {
            get { return valueForKey<ExceptionsAdditions>("exceptionsAdditions"); }
            set { takeValueForKey("exceptionsAdditions", value); }
        }
        public int? internalParentItemNum
        {
            get { return valueForKey<int?>("internalParentItemNum"); }
            set { takeValueForKey("internalParentItemNum", value); }
        }

        public int? internalParentOrderNum
        {
            get { return valueForKey<int?>("internalParentOrderNum"); }
            set { takeValueForKey("internalParentOrderNum", value); }
        }

        public int? internalParentTradeNum
        {
            get { return valueForKey<int?>("internalParentTradeNum"); }
            set { takeValueForKey("internalParentTradeNum", value); }
        }

        public string tradeModifiedInd
        {
            get { return valueForKey<string>("tradeModifiedInd"); }
            set { takeValueForKey("tradeModifiedInd", value); }
        }

        public string itemConfirmInd
        {
            get { return valueForKey<string>("itemConfirmInd"); }
            set { takeValueForKey("itemConfirmInd", value); }
        }
        public string clearingbroker
        {
            get { return valueForKey<string>("clearingbroker"); }
            set { takeValueForKey("clearingbroker", value); }
        }
        public int? clrServiceNum
        {
            get { return valueForKey<int?>("clrServiceNum"); }
            set { takeValueForKey("clrServiceNum", value); }
        }
        public Account clearingService
        {
            get { return valueForKey<Account>("clearingService"); }
            set { takeValueForKey("clearingService", value); }
        }

        public int? financeBankNum
        {
            get { return valueForKey<int?>("financeBankNum"); }
            set { takeValueForKey("financeBankNum", value); }
        }

        public int? exchBrkrNum
        {
            get { return valueForKey<int?>("exchBrkrNum"); }
            set { takeValueForKey("exchBrkrNum", value); }
        }
        public Account exchangeBroker
        {
            get { return valueForKey<Account>("exchangeBroker"); }
            set { takeValueForKey("exchangeBroker", value); }
        }
        public int? agreementNum
        {
            get { return valueForKey<int?>("agreementNum"); }
            set { takeValueForKey("agreementNum", value); }
        }

        public string activeStatusInd
        {
            get { return valueForKey<string>("activeStatusInd"); }
            set { takeValueForKey("activeStatusInd", value); }
        }

        public string formulaDescription
        {
            get { return valueForKey<string>("formulaDescription"); }
            set { takeValueForKey("formulaDescription", value); }
        }

        public string quoteTermDesc
        {
            get { return valueForKey<string>("quoteTermDesc"); }
            set { takeValueForKey("quoteTermDesc", value); }
        }

        public int? marketValue
        {
            get { return valueForKey<int?>("marketValue"); }
            set { takeValueForKey("marketValue", value); }
        }

        public int? includesExciseTaxInd
        {
            get { return valueForKey<int?>("includesExciseTaxInd"); }
            set { takeValueForKey("includesExciseTaxInd", value); }
        }

        public int? includesFuelTaxInd
        {
            get { return valueForKey<int?>("includesFuelTaxInd"); }
            set { takeValueForKey("includesFuelTaxInd", value); }
        }

        public Comment comment
        {
            get { return valueForKey<Comment>("comment"); }
            set { takeValueForKey("comment", value); }
        }

        public Account bookingCompany
        {
            get { return valueForKey<Account>("bookingCompany"); }
            set { takeValueForKey("bookingCompany", value); }
        }

        public Account broker
        {
            get { return valueForKey<Account>("broker"); }
            set { takeValueForKey("broker", value); }
        }

        public Calendar calendar
        {
            get { return valueForKey<Calendar>("calendar"); }
            set { takeValueForKey("calendar", value); }
        }

        public Commodity commodity
        {
            get { return valueForKey<Commodity>("commodity"); }
            set { takeValueForKey("commodity", value); }
        }

        public Uom contractQtyUom
        {
            get { return valueForKey<Uom>("contractQtyUom"); }
            set { takeValueForKey("contractQtyUom", value); }
        }

        public Uom committedQtyUom
        {
            get { return valueForKey<Uom>("committedQtyUom"); }
            set { takeValueForKey("committedQtyUom", value); }
        }

        public Commodity priceCurrency
        {
            get { return valueForKey<Commodity>("priceCurrency"); }
            set { takeValueForKey("priceCurrency", value); }
        }

        public Uom priceUom
        {
            get { return valueForKey<Uom>("priceUom"); }
            set { takeValueForKey("priceUom", value); }
        }

        public Uom pricedQtyUom
        {
            get { return valueForKey<Uom>("pricedQtyUom"); }
            set { takeValueForKey("pricedQtyUom", value); }
        }

        public RealPortfolio realPortfolio
        {
            get { return valueForKey<RealPortfolio>("realPortfolio"); }
            set { takeValueForKey("realPortfolio", value); }
        }

        public Market riskMarket
        {
            get { return valueForKey<Market>("riskMarket"); }
            set { takeValueForKey("riskMarket", value); }
        }

        public Market titleMarket
        {
            get { return valueForKey<Market>("titleMarket"); }
            set { takeValueForKey("titleMarket", value); }
        }

        public Transaction transaction
        {
            get { return valueForKey<Transaction>("transaction"); }
            set { takeValueForKey("transaction", value); }
        }

        public Commodity hedgeCurrency
        {
            get { return valueForKey<Commodity>("hedgeCurrency"); }
            set { takeValueForKey("hedgeCurrency", value); }
        }

        public Gtc gtc
        {
            get { return valueForKey<Gtc>("gtc"); }
            set { takeValueForKey("gtc", value); }
        }

        public Uom openQtyUom
        {
            get { return valueForKey<Uom>("openQtyUom"); }
            set { takeValueForKey("openQtyUom", value); }
        }

        public Uom schQtyUom
        {
            get { return valueForKey<Uom>("schQtyUom"); }
            set { takeValueForKey("schQtyUom", value); }
        }

        public Commodity brkrCommCurr
        {
            get { return valueForKey<Commodity>("brkrCommCurr"); }
            set { takeValueForKey("brkrCommCurr", value); }
        }

        public Uom brkrCommUom
        {
            get { return valueForKey<Uom>("brkrCommUom"); }
            set { takeValueForKey("brkrCommUom", value); }
        }

        public string purchaseSaleGroupCode
        {
            get { return valueForKey<string>("purchaseSaleGroupCode"); }
            set { takeValueForKey("purchaseSaleGroupCode", value); }
        }

        public PsGroupCodeRef psGroupCodeRef
        {
            get { return valueForKey<PsGroupCodeRef>("psGroupCodeRef"); }
            set { takeValueForKey("psGroupCodeRef", value); }
        }

        public Account financingBank
        {
            get { return valueForKey<Account>("financingBank"); }
            set { takeValueForKey("financingBank", value); }
        }

        public AccountAgreement accountAgreement
        {
            get { return valueForKey<AccountAgreement>("accountAgreement"); }
            set { takeValueForKey("accountAgreement", value); }
        }
        [DisplayHierarchy(Show = false)]
        public List<Cost> costs
        {
            get 
            {
                //if (hasPreFetched && this.hasFaults(cs.fw.eo.entity.keypath.KeyPath.TradeItem))
                if (hasPreFetched && this.hasFaultOrNotExist(cs.fw.eo.entity.keypath.KeyPath.TradeItem))
                    cs.fw.infra.service.ServiceUtil.FetchFault<TradeItem>(this, cs.fw.eo.entity.keypath.KeyPath.TradeItem.costs);
                return valueForKey<List<Cost>>("costs"); 
            }
            set { takeValueForKey("costs", value); }
        }

        //public List<RailcarIdentifierInfo> railcarIdentifierInfos
        //{
        //    get
        //    {
        //        if (hasPreFetched && this.hasFaults(cs.fw.eo.entity.keypath.KeyPath.TradeItem))
        //            cs.fw.infra.service.ServiceUtil.FetchFault<TradeItem>(this, cs.fw.eo.entity.keypath.KeyPath.TradeItem.railcarIdentifierInfos);
        //        return valueForKey<List<RailcarIdentifierInfo>>("railcarIdentifierInfos");
        //    }
        //    set { takeValueForKey("railcarIdentifierInfos", value); }
        //}

        //public TradeOrderRailcar tradeOrderRailcar
        //{
        //    get { return valueForKey<TradeOrderRailcar>("tradeOrderRailcar"); }
        //    set { takeValueForKey("tradeOrderRailcar", value); }
        //}

        //public List<RailcarPtpRate> railcarPtpRates
        //{
        //    get
        //    {
        //        if (hasPreFetched && this.hasFaults(cs.fw.eo.entity.keypath.KeyPath.TradeItem))
        //            cs.fw.infra.service.ServiceUtil.FetchFault<TradeItem>(this, cs.fw.eo.entity.keypath.KeyPath.TradeItem.railcarPtpRates);
        //        return valueForKey<List<RailcarPtpRate>>("railcarPtpRates");
        //    }
        //    set { takeValueForKey("railcarPtpRates", value); }
        //}

        //private IList<Cost> tiCosts;
        //public virtual IList<Cost> TiCosts {
        //    get
        //    {
        //        return tiCosts;
        //    }
        //    set
        //    {
        //        tiCosts = value;
        //    }
        //}

        
        public List<AssignTrade> assignTrades
        {
            get { return valueForKey<List<AssignTrade>>("assignTrades"); }
            set { takeValueForKey("assignTrades", value); }
        }
        //private IList<AssignTrade> tiAssignTrades;
        //public virtual IList<AssignTrade> TiAssignTrades
        //{
        //    get
        //    {
        //        return tiAssignTrades;
        //    }
        //    set
        //    {
        //        tiAssignTrades = value;
        //    }
        //}
        [DisplayHierarchy(Show = false)]
        public List<RCAssignTrade> rcAssignTrades
        {
            get { return valueForKey<List<RCAssignTrade>>("rcAssignTrades"); }
            set { takeValueForKey("rcAssignTrades", value); }
        }

        public override string ToString()
        {
            return "Trade : " + tradeNum + "/" + orderNum + "/" + itemNum;
        }

        //code added by Padma Rao on 26 June 2009

        public RealPortfolio transferedPortfolio
        {
            get { return valueForKey<RealPortfolio>("transferedPortfolio"); }
            set { takeValueForKey("transferedPortfolio", value); }
        }

        //end of code added by Padma Rao on 26 June 2009

        //code added by Padma Rao on 8 July 2009 to add the parentItem property

        public TradeItem parentItem
        {
            get { return valueForKey<TradeItem>("parentItem"); }
            set { takeValueForKey("parentItem", value); }
        }

        //code added by Padma Rao on 8 July 2009



        //code added by Padma Rao on 21 July 2009 to add parcels property
        [DisplayHierarchy(Show = false)]
        public List<Parcel> parcels
        {
            get
            {
                return valueForKey<List<Parcel>>("parcels");
            }
            set
            {
                takeValueForKey("parcels", value);
            }
        }

        //end of code added by Padma Rao on 21 July 2009

        [DisplayHierarchy(Show = false)]
        public List<AllocationItem> allocationItems
        {
            get
            {
                return valueForKey<List<AllocationItem>>("allocationItems");
            }
            set
            {
                takeValueForKey("allocationItems", value);
            }
        }
        //Code added by Srikanth Rao on 5 June 2010 to add property TradeDistributions.
        //To fix issue number 909039 we have modified the DisplayHierarchy.show to false
        //so that it won't appear in DisplayHierarchy on TradeItems ribbon bar
        [DisplayHierarchy(Show = false)]
        public List<TradeItemDist> distributions
        {
            get
            {
                return valueForKey<List<TradeItemDist>>("distributions");
            }
            set
            {
                takeValueForKey("distributions", value);
            }
        }

        //private IList<TradeItemDist> tiDistributions;
        //public virtual IList<TradeItemDist> TiDistributions
        //{
        //    get
        //    {
        //        return tiDistributions;
        //    }
        //    set
        //    {
        //        tiDistributions = value;
        //    }
        //}
        //Code ended by Srikanth Rao on 5 June 2010
        //code added by Padma Rao on 23 July 2009 to add the property TradeFormulas
        public List<TradeFormula> tradeFormulas
        {
            get
            {
                return valueForKey<List<TradeFormula>>("tradeFormulas");
            }
            set
            {
                takeValueForKey("tradeFormulas", value);
            }
        }

        [DisplayHierarchy(Show = false)]
        public List<TiFieldModified> tradeItemModFields
        {
            get
            {
                return valueForKey<List<TiFieldModified>>("tradeItemModFields");
            }
            set
            {
                takeValueForKey("tradeItemModFields", value);
            }
        }

        public TradeItemDist firstRealDeliveryDist
        {
            get
            {
                return valueForKey<TradeItemDist>("firstRealDeliveryDist");
            }
            set
            {
                takeValueForKey("firstRealDeliveryDist", value);
            }
        }
        //end of code added by Padma Rao on 23 July 2009

        //code added by Padma Rao on 14 Aug 2009 to add the new property totalCommittedQty

        public decimal? totalCommittedQty
        {
            get
            {
                return valueForKey<decimal?>("totalCommittedQty");
            }
            set
            {
                takeValueForKey("totalCommittedQty", value);
            }
        }

        //end of code added by Padma Rao on 14 Aug 2009
        //code added by Padma Rao on 5 Nov 2009 to add teh property deliveryTerm
        public DeliveryTerm deliveryTerm
        {
            get
            {
                return valueForKey<DeliveryTerm>("deliveryTerm");
            }
            set
            {
                takeValueForKey("deliveryTerm", value);
            }
        }

        //end of code added by Padma Rao on 5 Nov 2009
        public Country originCountry
        {
            get { return valueForKey<Country>("originCountry"); }
            set { takeValueForKey("originCountry", value); }
        }

        //Properties for derived fields
        public decimal? contrQtyAmountPerLife
        {
            get { return valueForKey<decimal?>("contrQtyAmountPerLife"); }
            set { takeValueForKey("contrQtyAmountPerLife", value); }
        }

        public decimal? openQtyAmountPerLife
        {
            get { return valueForKey<decimal?>("openQtyAmountPerLife"); }
            set { takeValueForKey("openQtyAmountPerLife", value); }
        }

        public decimal? totalSchQtyAmountPerLife
        {
            get { return valueForKey<decimal?>("totalSchQtyAmountPerLife"); }
            set { takeValueForKey("totalSchQtyAmountPerLife", value); }
        }

        public decimal? contrQtyAmountPerDay
        {
            get { return valueForKey<decimal?>("contrQtyAmountPerDay"); }
            set { takeValueForKey("contrQtyAmountPerDay", value); }
        }

        public decimal? openQtyAmountPerDay
        {
            get { return valueForKey<decimal?>("openQtyAmountPerDay"); }
            set { takeValueForKey("openQtyAmountPerDay", value); }
        }

        public decimal? totalSchQtyAmountPerDay
        {
            get { return valueForKey<decimal?>("totalSchQtyAmountPerDay"); }
            set { takeValueForKey("totalSchQtyAmountPerDay", value); }
        }

        public decimal? strikePrice
        {
            get { return valueForKey<decimal?>("strikePrice"); }
            set { takeValueForKey("strikePrice", value); }
        }

        public decimal? earliestMarketPrice
        {
            get { return valueForKey<decimal?>("earliestMarketPrice"); }
            set { takeValueForKey("earliestMarketPrice", value); }
        }

        public decimal? latestMarketPrice
        {
            get { return valueForKey<decimal?>("latestMarketPrice"); }
            set { takeValueForKey("latestMarketPrice", value); }
        }

        public decimal? earliestPl
        {
            get { return valueForKey<decimal?>("earliestPl"); }
            set { takeValueForKey("earliestPl", value); }
        }

        public decimal? latestPl
        {
            get { return valueForKey<decimal?>("latestPl"); }
            set { takeValueForKey("latestPl", value); }
        }

        public decimal? baseDensity
        {
            get { return valueForKey<decimal?>("baseDensity"); }
            set { takeValueForKey("baseDensity", value); }
        }

        public decimal? primarySettlement
        {
            get { return valueForKey<decimal?>("primarySettlement"); }
            set { takeValueForKey("primarySettlement", value); }
        }

        public string contractQtyPeriodicityString
        {
            get { return valueForKey<string>("contractQtyPeriodicityString"); }
            set { takeValueForKey("contractQtyPeriodicityString", value); }
        }
        
        public string splitCycleOptString
        {
            get { return valueForKey<string>("splitCycleOptString"); }
            set { takeValueForKey("splitCycleOptString", value); }
        }

        public string cycleYearString
        {
            get { return valueForKey<string>("cycleYearString"); }
            set { takeValueForKey("cycleYearString", value); }
        }

        public string poolingPortDescription
        {
            get { return valueForKey<string>("poolingPortDescription"); }
            set { takeValueForKey("poolingPortDescription", value); }
        }

        public string poolingPortIndicator
        {
            get { return valueForKey<string>("poolingPortIndicator"); }
            set { takeValueForKey("poolingPortIndicator", value); }
        }

        public string toleranceDesc
        {
            get { return valueForKey<string>("toleranceDesc"); }
            set { takeValueForKey("toleranceDesc", value); }
        }

        public string densityAdjusted
        {
            get { return valueForKey<string>("densityAdjusted"); }
            set { takeValueForKey("densityAdjusted", value); }
        }

        public DateTime? earliestAccumStartDate
        {
            get { return valueForKey<DateTime?>("earliestAccumStartDate"); }
            set { takeValueForKey("earliestAccumStartDate", value); }
        }

        public DateTime? latestAccumEndDate
        {
            get { return valueForKey<DateTime?>("latestAccumEndDate"); }
            set { takeValueForKey("latestAccumEndDate", value); }
        }

        public DateTime? earliestPLDate
        {
            get { return valueForKey<DateTime?>("earliestPLDate"); }
            set { takeValueForKey("earliestPLDate", value); }
        }

        public DateTime? latestPLDate
        {
            get { return valueForKey<DateTime?>("latestPLDate"); }
            set { takeValueForKey("latestPLDate", value); }
        }

        public string displayRoll
        {
            get { return valueForKey<string>("displayRoll"); }
            set { takeValueForKey("displayRoll", value); }
        }

        public string displayAllQuotesReqdIndicatorOnFormula
        {
            get { return valueForKey<string>("displayAllQuotesReqdIndicatorOnFormula"); }
            set { takeValueForKey("displayAllQuotesReqdIndicatorOnFormula", value); }
        }

        public DateTime? earliestQuoteDate
        {
            get { return valueForKey<DateTime?>("earliestQuoteDate"); }
            set { takeValueForKey("earliestQuoteDate", value); }
        }

        public DateTime? latestQuoteDate
        {
            get { return valueForKey<DateTime?>("latestQuoteDate"); }
            set { takeValueForKey("latestQuoteDate", value); }
        }

        public TradeItemStorage tradeItemStorage
        {
            get { return valueForKey<TradeItemStorage>("tradeItemStorage"); }
            set { takeDerivedValueForKey("tradeItemStorage", value); }
        }

        public TradeItemBunker tradeItemBunker
        {
            get { return valueForKey<TradeItemBunker>("tradeItemBunker"); }
            set { takeDerivedValueForKey("tradeItemBunker", value); }
        }

        public TradeItemTransport tradeItemTransport
        {
            get { return valueForKey<TradeItemTransport>("tradeItemTransport"); }
            set { takeDerivedValueForKey("tradeItemTransport", value); }
        }
        public string isClearedInd
        {
            get { return valueForKey<string>("isClearedInd"); }
            set { takeValueForKey("isClearedInd", value); }
        }

        //code added by Padma Rao on 22 July 2010 to add the  properties cannetOut and canBookOut
        public virtual int? canNetOut
        {
            get
            {
                return valueForKey<int?>("canNetOut");
            }
            set
            {
                takeValueForKey("canNetOut", value);
            }
        }

        public virtual int? canBookOut
        {
            get
            {
                return valueForKey<int?>("canBookOut");
            }
            set
            {
                takeValueForKey("canBookOut", value);
            }
        }
        //end of code added by padma Rao on 22 July 2010

        public string tradeItemShipmentList
        {
            get { return valueForKey<string>("tradeItemShipmentList"); }
            set { takeValueForKey("tradeItemShipmentList", value); }
        }
        public bool? isLoadCreditRequired
        {
            get { return valueForKey<bool?>("isLoadCreditRequired"); }
            set { takeValueForKey("isLoadCreditRequired", value); }
        }

        public string validateCreditApprovedLoadingonTI
        {
            get { return valueForKey<string>("validateCreditApprovedLoadingonTI"); }
            set { takeValueForKey("validateCreditApprovedLoadingonTI", value); }
        }

        //code added by Padma Rao on 8 Oct 2010 based on issue 840992
        [DisplayHierarchy(Show = false)]
        public List<EventPriceTerm> eventPriceTerms
        {
            get
            {
                return valueForKey<List<EventPriceTerm>>("eventPriceTerms");
            }
            set
            {
                takeValueForKey("eventPriceTerms", value);
            }
        }
        //end of code added by Padma Rao on 8 Oct 2010

        //code added by Satish on Feb 2011 based on issue 1248644
        public bool isPortfolioLocked
        {
            get { return valueForKey<bool>("isPortfolioLocked"); }
            set { takeValueForKey("isPortfolioLocked", value); }
        }

        
        //public override string EntityName
        //{
        //    get { return "TradeItem"; }
        //}
        //code commented by Padma Rao on 18 Dec 2012 based on issue 1362672
        //public bool isPortfolioEditable
        //{
        //    get { return valueForKey<bool>("isPortfolioEditable"); }
        //    set { takeValueForKey("isPortfolioEditable", value); }
        //}
        //public bool isRiskAndTitleEditable
        //{
        //    get { return valueForKey<bool>("isRiskAndTitleEditable"); }
        //    set { takeValueForKey("isRiskAndTitleEditable", value); }
        //}
        //end of code commented by Padma Rao on 18 Dec 2012
        //end of code added by Satish on 16 Feb 2011

        public decimal? totalParcelsNominQty
        {
            get { return valueForKey<decimal?>("totalParcelsNominQty"); }
            set { takeValueForKey("totalParcelsNominQty", value); }
        }

        public AccountContact brokerContact
        {
            get { return valueForKey<AccountContact>("brokerContact"); }
            set { takeValueForKey("brokerContact", value); }
        }

        public decimal? totalNomQtyMax
        {
            get { return valueForKey<decimal?>("totalNomQtyMax"); }
            set { takeValueForKey("totalNomQtyMax", value); }
        }

        public decimal? totalNomQtyMin
        {
            get { return valueForKey<decimal?>("totalNomQtyMin"); }
            set { takeValueForKey("totalNomQtyMin", value); }
        }

        public string useMktFormulaForPl
        {
            get { return valueForKey<string>("useMktFormulaForPl"); }
            set { takeValueForKey("useMktFormulaForPl", value); }
        }


        bool isDetailDataLoaded = false;
        List<LazyLoad> _lazyLoads = null;
        [DisplayHierarchy(Show = false)]
        public List<LazyLoad> lazyLoad
        {
            get
            {
                if (isDetailDataLoaded == false)
                {
                    if (_lazyLoads == null)
                    {
                        _lazyLoads = new List<LazyLoad>();
                        _lazyLoads.Add(new LazyLoad());
                    }
                    return _lazyLoads;
                }
                else
                    return null;
            }
        }

        public override void WriteElements(XmlWriter writer, int serializationDepth)
        {
            try
            {
                base.WriteElements(writer, serializationDepth);
                string ns = "http://tradecapture/entity";

                //if (RecordKey != null && !RecordKey.Equals(string.Empty))
                //{
                //    writer.WriteStartElement("RecordKey");
                //    writer.WriteString(RecordKey);
                //    writer.WriteEndElement();
                //}

                if (tradeNum.HasValue)
                {
                    writer.WriteStartElement("tradeNum");
                    writer.WriteString(tradeNum.Value.ToString());
                    writer.WriteEndElement();
                }
                if (orderNum.HasValue)
                {
                    writer.WriteStartElement("orderNum");
                    writer.WriteString(orderNum.Value.ToString());
                    writer.WriteEndElement();
                }
                if (itemNum.HasValue)
                {
                    writer.WriteStartElement("itemNum");
                    writer.WriteString(itemNum.Value.ToString());
                    writer.WriteEndElement();
                }

                if (CanSerializeChildObjects && tradeOrder != null)
                {
                    //writer.WriteStartElement("tradeOrder"); 
                    //writer.WriteAttributeString("xsi", "type", ns, tradeOrder.GetType().Name);
                    //tradeOrder.WriteElements(writer, serializationDepth + 1);
                    //writer.WriteEndElement();
                }
                if (cmdtyCode != null && !cmdtyCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("cmdtyCode");
                    writer.WriteString(cmdtyCode);
                    writer.WriteEndElement();
                }

                if (isLcAssigned != null && !isLcAssigned.Equals(string.Empty))
                {
                    writer.WriteStartElement("isLcAssigned");
                    writer.WriteString(isLcAssigned);
                    writer.WriteEndElement();
                }

                if (isRcAssigned != null && !isRcAssigned.Equals(string.Empty))
                {
                    writer.WriteStartElement("isRcAssigned");
                    writer.WriteString(isRcAssigned);
                    writer.WriteEndElement();
                }

                if (itemStatusCode != null && !itemStatusCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("itemStatusCode");
                    writer.WriteString(itemStatusCode);
                    writer.WriteEndElement();
                }

                if (itemType != null && !itemType.Equals(string.Empty))
                {
                    writer.WriteStartElement("itemType");
                    writer.WriteString(itemType);
                    writer.WriteEndElement();
                }

                if (pSInd != null && !pSInd.Equals(string.Empty))
                {
                    writer.WriteStartElement("pSInd");
                    writer.WriteString(pSInd);
                    writer.WriteEndElement();
                }

                if (avgPrice.HasValue)
                {
                    writer.WriteStartElement("avgPrice");
                    writer.WriteString(avgPrice.Value.ToString());
                    writer.WriteEndElement();
                }
                if (bookingCompNum.HasValue)
                {
                    writer.WriteStartElement("bookingCompNum");
                    writer.WriteString(bookingCompNum.Value.ToString());
                    writer.WriteEndElement();
                }
                if (contrQty.HasValue)
                {
                    writer.WriteStartElement("contrQty");
                    writer.WriteString(contrQty.Value.ToString());
                    writer.WriteEndElement();
                }

                if (contrQtyPeriodicity != null && !contrQtyPeriodicity.Equals(string.Empty))
                {
                    writer.WriteStartElement("contrQtyPeriodicity");
                    writer.WriteString(contrQtyPeriodicity);
                    writer.WriteEndElement();
                }

                if (contrQtyUomCode != null && !contrQtyUomCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("contrQtyUomCode");
                    writer.WriteString(contrQtyUomCode);
                    writer.WriteEndElement();
                }

                if (estimateInd != null && !estimateInd.Equals(string.Empty))
                {
                    writer.WriteStartElement("estimateInd");
                    writer.WriteString(estimateInd);
                    writer.WriteEndElement();
                }

                if (formulaInd != null && !formulaInd.Equals(string.Empty))
                {
                    writer.WriteStartElement("formulaInd");
                    writer.WriteString(formulaInd);
                    writer.WriteEndElement();
                }

                if (gtcCode != null && !gtcCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("gtcCode");
                    writer.WriteString(gtcCode);
                    writer.WriteEndElement();
                }

                if (openQty.HasValue)
                {
                    writer.WriteStartElement("openQty");
                    writer.WriteString(openQty.Value.ToString());
                    writer.WriteEndElement();
                }

                if (openQtyUomCode != null && !openQtyUomCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("openQtyUomCode");
                    writer.WriteString(openQtyUomCode);
                    writer.WriteEndElement();
                }

                if (priceCurrCode != null && !priceCurrCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("priceCurrCode");
                    writer.WriteString(priceCurrCode);
                    writer.WriteEndElement();
                }

                if (priceUomCode != null && !priceUomCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("priceUomCode");
                    writer.WriteString(priceUomCode);
                    writer.WriteEndElement();
                }

                if (pricedQtyUomCode != null && !pricedQtyUomCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("pricedQtyUomCode");
                    writer.WriteString(pricedQtyUomCode);
                    writer.WriteEndElement();
                }

                if (b2bTradeItem != null && !b2bTradeItem.Equals(string.Empty))
                {
                    writer.WriteStartElement("b2bTradeItem");
                    writer.WriteString(b2bTradeItem);
                    writer.WriteEndElement();
                }
                if (realPortNum.HasValue)
                {
                    writer.WriteStartElement("realPortNum");
                    writer.WriteString(realPortNum.Value.ToString());
                    writer.WriteEndElement();
                }

                if (riskMktCode != null && !riskMktCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("riskMktCode");
                    writer.WriteString(riskMktCode);
                    writer.WriteEndElement();
                }

                if (schQtyUomCode != null && !schQtyUomCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("schQtyUomCode");
                    writer.WriteString(schQtyUomCode);
                    writer.WriteEndElement();
                }

                if (schedStatus.HasValue)
                {
                    writer.WriteStartElement("schedStatus");
                    writer.WriteString(schedStatus.Value.ToString());
                    writer.WriteEndElement();
                }

                if (titleMktCode != null && !titleMktCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("titleMktCode");
                    writer.WriteString(titleMktCode);
                    writer.WriteEndElement();
                }

                if (totalPricedQty.HasValue)
                {
                    writer.WriteStartElement("totalPricedQty");
                    writer.WriteString(totalPricedQty.Value.ToString());
                    writer.WriteEndElement();
                }
                if (totalSchQty.HasValue)
                {
                    writer.WriteStartElement("totalSchQty");
                    writer.WriteString(totalSchQty.Value.ToString());
                    writer.WriteEndElement();
                }
                if (transId.HasValue)
                {
                    writer.WriteStartElement("transId");
                    writer.WriteString(transId.Value.ToString());
                    writer.WriteEndElement();
                }
                if (dischPortLocCode != null && !dischPortLocCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("dischPortLocCode");
                    writer.WriteString(dischPortLocCode);
                    writer.WriteEndElement();
                }
                if (loadPortLocCode != null && !loadPortLocCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("loadPortLocCode");
                    writer.WriteString(loadPortLocCode);
                    writer.WriteEndElement();
                }
                if (accumPeriodicity != null && !accumPeriodicity.Equals(string.Empty))
                {
                    writer.WriteStartElement("accumPeriodicity");
                    writer.WriteString(accumPeriodicity);
                    writer.WriteEndElement();
                }
                if (amendCreationDate.HasValue && !amendCreationDate.Value.Equals(DateTime.MinValue))
                {
                    writer.WriteStartElement("amendCreationDate");
                    if (IsExportToExcelRequested == true)
                        writer.WriteValue(amendCreationDate.Value.ToString("O").Substring(0, 19));
                    else
                        writer.WriteValue(amendCreationDate.Value.ToString("O").Substring(0, 27));
                    writer.WriteEndElement();
                }
                if (amendEffectEndDate.HasValue && !amendEffectEndDate.Value.Equals(DateTime.MinValue))
                {
                    writer.WriteStartElement("amendEffectEndDate");
                    if (IsExportToExcelRequested == true)
                        writer.WriteValue(amendEffectEndDate.Value.ToString("O").Substring(0, 19));
                    else
                        writer.WriteValue(amendEffectEndDate.Value.ToString("O").Substring(0, 27));
                    writer.WriteEndElement();
                }
                if (amendEffectStartDate.HasValue && !amendEffectStartDate.Value.Equals(DateTime.MinValue))
                {
                    writer.WriteStartElement("amendEffectStartDate");
                    if (IsExportToExcelRequested == true)
                        writer.WriteValue(amendEffectStartDate.Value.ToString("O").Substring(0, 19));
                    else
                        writer.WriteValue(amendEffectStartDate.Value.ToString("O").Substring(0, 27));
                    writer.WriteEndElement();
                }
                if (amendNum.HasValue)
                {
                    writer.WriteStartElement("amendNum");
                    writer.WriteString(amendNum.Value.ToString());
                    writer.WriteEndElement();
                }
                if (billingType != null && !billingType.Equals(string.Empty))
                {
                    writer.WriteStartElement("billingType");
                    writer.WriteString(billingType);
                    writer.WriteEndElement();
                }
                if (brkrCommAmt.HasValue)
                {
                    writer.WriteStartElement("brkrCommAmt");
                    writer.WriteString(brkrCommAmt.Value.ToString());
                    writer.WriteEndElement();
                }
                if (brkrCommCurrCode != null && !brkrCommCurrCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("brkrCommCurrCode");
                    writer.WriteString(brkrCommCurrCode);
                    writer.WriteEndElement();
                }
                if (brkrCommUomCode != null && !brkrCommUomCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("brkrCommUomCode");
                    writer.WriteString(brkrCommUomCode);
                    writer.WriteEndElement();
                }
                if (brkrContNum.HasValue)
                {
                    writer.WriteStartElement("brkrContNum");
                    writer.WriteString(brkrContNum.Value.ToString());
                    writer.WriteEndElement();
                }
                if (brkrNum.HasValue)
                {
                    writer.WriteStartElement("brkrNum");
                    writer.WriteString(brkrNum.Value.ToString());
                    writer.WriteEndElement();
                }
                if (brkrRefNum != null && !brkrRefNum.Equals(string.Empty))
                {
                    writer.WriteStartElement("brkrRefNum");
                    writer.WriteString(brkrRefNum);
                    writer.WriteEndElement();
                }
                if (cmntNum.HasValue)
                {
                    writer.WriteStartElement("cmntNum");
                    writer.WriteString(cmntNum.Value.ToString());
                    writer.WriteEndElement();
                }
                if (futTraderInit != null && !futTraderInit.Equals(string.Empty))
                {
                    writer.WriteStartElement("futTraderInit");
                    writer.WriteString(futTraderInit);
                    writer.WriteEndElement();
                }
                if (hedgeCurrCode != null && !hedgeCurrCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("hedgeCurrCode");
                    writer.WriteString(hedgeCurrCode);
                    writer.WriteEndElement();
                }
                if (hedgeMultiDivInd != null && !hedgeMultiDivInd.Equals(string.Empty))
                {
                    writer.WriteStartElement("hedgeMultiDivInd");
                    writer.WriteString(hedgeMultiDivInd);
                    writer.WriteEndElement();
                }
                if (hedgePosInd != null && !hedgePosInd.Equals(string.Empty))
                {
                    writer.WriteStartElement("hedgePosInd");
                    writer.WriteString(hedgePosInd);
                    writer.WriteEndElement();
                }
                if (hedgeRate.HasValue)
                {
                    writer.WriteStartElement("hedgeRate");
                    writer.WriteString(hedgeRate.Value.ToString());
                    writer.WriteEndElement();
                }
                if (idmsAcctAlloc != null && !idmsAcctAlloc.Equals(string.Empty))
                {
                    writer.WriteStartElement("idmsAcctAlloc");
                    writer.WriteString(idmsAcctAlloc);
                    writer.WriteEndElement();
                }
                if (idmsBbRefNum != null && !idmsBbRefNum.Equals(string.Empty))
                {
                    writer.WriteStartElement("idmsBbRefNum");
                    writer.WriteString(idmsBbRefNum);
                    writer.WriteEndElement();
                }
                if (idmsContrNum != null && !idmsContrNum.Equals(string.Empty))
                {
                    writer.WriteStartElement("idmsContrNum");
                    writer.WriteString(idmsContrNum);
                    writer.WriteEndElement();
                }
                if (idmsProfitCenter != null && !idmsProfitCenter.Equals(string.Empty))
                {
                    writer.WriteStartElement("idmsProfitCenter");
                    writer.WriteString(idmsProfitCenter);
                    writer.WriteEndElement();
                }
                if (parentItemNum.HasValue)
                {
                    writer.WriteStartElement("parentItemNum");
                    writer.WriteString(parentItemNum.Value.ToString());
                    writer.WriteEndElement();
                }
                if (poolingPortInd != null && !poolingPortInd.Equals(string.Empty))
                {
                    writer.WriteStartElement("poolingPortInd");
                    writer.WriteString(poolingPortInd);
                    writer.WriteEndElement();
                }
                if (poolingPortNum.HasValue)
                {
                    writer.WriteStartElement("poolingPortNum");
                    writer.WriteString(poolingPortNum.Value.ToString());
                    writer.WriteEndElement();
                }
                if (poolingType != null && !poolingType.Equals(string.Empty))
                {
                    writer.WriteStartElement("poolingType");
                    writer.WriteString(poolingType);
                    writer.WriteEndElement();
                }
                if (recapItemNum.HasValue)
                {
                    writer.WriteStartElement("recapItemNum");
                    writer.WriteString(recapItemNum.Value.ToString());
                    writer.WriteEndElement();
                }
                if (stripItemStatus != null && !stripItemStatus.Equals(string.Empty))
                {
                    writer.WriteStartElement("stripItemStatus");
                    writer.WriteString(stripItemStatus);
                    writer.WriteEndElement();
                }
                if (summaryItemNum.HasValue)
                {
                    writer.WriteStartElement("summaryItemNum");
                    writer.WriteString(summaryItemNum.Value.ToString());
                    writer.WriteEndElement();
                }
                if (tradingPrd != null && !tradingPrd.Equals(string.Empty))
                {
                    writer.WriteStartElement("tradingPrd");
                    writer.WriteString(tradingPrd);
                    writer.WriteEndElement();
                }
                if (uomConvRate.HasValue)
                {
                    writer.WriteStartElement("uomConvRate");
                    writer.WriteString(uomConvRate.Value.ToString());
                    writer.WriteEndElement();
                }
                if (maxAccumNum.HasValue)
                {
                    writer.WriteStartElement("maxAccumNum");
                    writer.WriteString(maxAccumNum.Value.ToString());
                    writer.WriteEndElement();
                }
                if (formulaDeclarDate.HasValue && !formulaDeclarDate.Value.Equals(DateTime.MinValue))
                {
                    writer.WriteStartElement("formulaDeclarDate");
                    if (IsExportToExcelRequested == true)
                        writer.WriteValue(formulaDeclarDate.Value.ToString("O").Substring(0, 19));
                    else
                        writer.WriteValue(formulaDeclarDate.Value.ToString("O").Substring(0, 27));
                    writer.WriteEndElement();
                }
                if (purchasingGroup != null && !purchasingGroup.Equals(string.Empty))
                {
                    writer.WriteStartElement("purchasingGroup");
                    writer.WriteString(purchasingGroup);
                    writer.WriteEndElement();
                }
                if (originCountryCode != null && !originCountryCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("originCountryCode");
                    writer.WriteString(originCountryCode);
                    writer.WriteEndElement();
                }
                if (excpAddnsCode != null && !excpAddnsCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("excpAddnsCode");
                    writer.WriteString(excpAddnsCode);
                    writer.WriteEndElement();
                }
                if (internalParentItemNum.HasValue)
                {
                    writer.WriteStartElement("internalParentItemNum");
                    writer.WriteString(internalParentItemNum.Value.ToString());
                    writer.WriteEndElement();
                }
                if (internalParentOrderNum.HasValue)
                {
                    writer.WriteStartElement("internalParentOrderNum");
                    writer.WriteString(internalParentOrderNum.Value.ToString());
                    writer.WriteEndElement();
                }
                if (internalParentTradeNum.HasValue)
                {
                    writer.WriteStartElement("internalParentTradeNum");
                    writer.WriteString(internalParentTradeNum.Value.ToString());
                    writer.WriteEndElement();
                }
                if (tradeModifiedInd != null && !tradeModifiedInd.Equals(string.Empty))
                {
                    writer.WriteStartElement("tradeModifiedInd");
                    writer.WriteString(tradeModifiedInd);
                    writer.WriteEndElement();
                }
                if (itemConfirmInd != null && !itemConfirmInd.Equals(string.Empty))
                {
                    writer.WriteStartElement("itemConfirmInd");
                    writer.WriteString(itemConfirmInd);
                    writer.WriteEndElement();
                }
                if (financeBankNum.HasValue)
                {
                    writer.WriteStartElement("financeBankNum");
                    writer.WriteString(financeBankNum.Value.ToString());
                    writer.WriteEndElement();
                }
                if (exchBrkrNum.HasValue)
                {
                    writer.WriteStartElement("exchBrkrNum");
                    writer.WriteString(exchBrkrNum.Value.ToString());
                    writer.WriteEndElement();
                }
                if (agreementNum.HasValue)
                {
                    writer.WriteStartElement("agreementNum");
                    writer.WriteString(agreementNum.Value.ToString());
                    writer.WriteEndElement();
                }
                if (activeStatusInd != null && !activeStatusInd.Equals(string.Empty))
                {
                    writer.WriteStartElement("activeStatusInd");
                    writer.WriteString(activeStatusInd);
                    writer.WriteEndElement();
                }

                //code added by Padma Rao on 14 Aug 2009 to add the new property totalCommittedQty

                if (totalCommittedQty.HasValue)
                {
                    writer.WriteStartElement("totalCommittedQty");
                    writer.WriteString(totalCommittedQty.Value.ToString());
                    writer.WriteEndElement();
                }

                if (calendarCode != null && !calendarCode.Equals(string.Empty))
                {
                    writer.WriteStartElement("calendarCode");
                    writer.WriteString(calendarCode);
                    writer.WriteEndElement();
                }

                //end of code added by Padma Rao on 14 Aug 2009
                //writer.WriteElementString("marketValue", marketValue.HasValue ? marketValue.Value.ToString() : "");
                //writer.WriteElementString("includesExciseTaxInd", includesExciseTaxInd.HasValue ? includesExciseTaxInd.Value.ToString() : "");
                //writer.WriteElementString("includesFuelTaxInd", includesFuelTaxInd.HasValue ? includesFuelTaxInd.Value.ToString() : "");
                //if (CanSerializeChildObjects && comment != null)
                //{
                //    writer.WriteStartElement("comment");
                //    writer.WriteAttributeString("xsi", "type", ns, typeof(Comment).Name);
                //    comment.WriteElements(writer, serializationDepth + 1);
                //    writer.WriteEndElement();
                //}
                //if (CanSerializeChildObjects && bookingCompany != null)
                //{
                //    writer.WriteStartElement("bookingCompany");
                //    writer.WriteAttributeString("xsi", "type", ns, typeof(Account).Name);
                //    bookingCompany.WriteElements(writer, serializationDepth + 1);
                //    writer.WriteEndElement();
                //}
                //if (CanSerializeChildObjects && broker != null)
                //{
                //    writer.WriteStartElement("broker");
                //    writer.WriteAttributeString("xsi", "type", ns, typeof(Account).Name);
                //    broker.WriteElements(writer, serializationDepth + 1);
                //    writer.WriteEndElement();
                //}
                //if (CanSerializeChildObjects && commodity != null)
                //{
                //    writer.WriteStartElement("commodity");
                //    writer.WriteAttributeString("xsi", "type", ns, typeof(Commodity).Name);
                //    commodity.WriteElements(writer, serializationDepth + 1);
                //    writer.WriteEndElement();
                //}
                //if (CanSerializeChildObjects && contractQtyUom != null)
                //{
                //    writer.WriteStartElement("contractQtyUom");
                //    writer.WriteAttributeString("xsi", "type", ns, typeof(Uom).Name);
                //    contractQtyUom.WriteElements(writer, serializationDepth + 1);
                //    writer.WriteEndElement();
                //}
                //if (CanSerializeChildObjects && priceCurrency != null)
                //{
                //    writer.WriteStartElement("priceCurrency");
                //    writer.WriteAttributeString("xsi", "type", ns, typeof(Commodity).Name);
                //    priceCurrency.WriteElements(writer, serializationDepth + 1);
                //    writer.WriteEndElement();
                //}
                //if (CanSerializeChildObjects && priceUom != null)
                //{
                //    writer.WriteStartElement("priceUom");
                //    writer.WriteAttributeString("xsi", "type", ns, typeof(Uom).Name);
                //    priceUom.WriteElements(writer, serializationDepth + 1);
                //    writer.WriteEndElement();
                //}
                //if (CanSerializeChildObjects && pricedQtyUom != null)
                //{
                //    writer.WriteStartElement("pricedQtyUom");
                //    writer.WriteAttributeString("xsi", "type", ns, typeof(Uom).Name);
                //    pricedQtyUom.WriteElements(writer, serializationDepth + 1);
                //    writer.WriteEndElement();
                //}
                ////realPortfolio
                //if (CanSerializeChildObjects && riskMarket != null)
                //{
                //    writer.WriteStartElement("riskMarket");
                //    writer.WriteAttributeString("xsi", "type", ns, typeof(Market).Name);
                //    riskMarket.WriteElements(writer, serializationDepth + 1);
                //    writer.WriteEndElement();
                //}
                //if (CanSerializeChildObjects && titleMarket != null)
                //{
                //    writer.WriteStartElement("titleMarket");
                //    writer.WriteAttributeString("xsi", "type", ns, typeof(Market).Name);
                //    titleMarket.WriteElements(writer, serializationDepth + 1);
                //    writer.WriteEndElement();
                //}
                //if (CanSerializeChildObjects && transaction != null)
                //{
                //    writer.WriteStartElement("transaction");
                //    writer.WriteAttributeString("xsi", "type", ns, typeof(Transaction).Name);
                //    transaction.WriteElements(writer, serializationDepth + 1);
                //    writer.WriteEndElement();
                //}
                //if (CanSerializeChildObjects && hedgeCurrency != null)
                //{
                //    writer.WriteStartElement("hedgeCurrency");
                //    writer.WriteAttributeString("xsi", "type", ns, typeof(Commodity).Name);
                //    hedgeCurrency.WriteElements(writer, serializationDepth + 1);
                //    writer.WriteEndElement();
                //}
                ////gtc
                //if (CanSerializeChildObjects && openQtyUom != null)
                //{
                //    writer.WriteStartElement("openQtyUom");
                //    writer.WriteAttributeString("xsi", "type", ns, typeof(Uom).Name);
                //    openQtyUom.WriteElements(writer, serializationDepth + 1);
                //    writer.WriteEndElement();
                //}
                //if (CanSerializeChildObjects && schQtyUom != null)
                //{
                //    writer.WriteStartElement("schQtyUom");
                //    writer.WriteAttributeString("xsi", "type", ns, typeof(Uom).Name);
                //    schQtyUom.WriteElements(writer, serializationDepth + 1);
                //    writer.WriteEndElement();
                //}
                //if (CanSerializeChildObjects && brkrCommCurr != null)
                //{
                //    writer.WriteStartElement("brkrCommCurr");
                //    writer.WriteAttributeString("xsi", "type", ns, typeof(Commodity).Name);
                //    brkrCommCurr.WriteElements(writer, serializationDepth + 1);
                //    writer.WriteEndElement();
                //}
                //if (CanSerializeChildObjects && brkrCommUom != null)
                //{
                //    writer.WriteStartElement("brkrCommUom");
                //    writer.WriteAttributeString("xsi", "type", ns, typeof(Uom).Name);
                //    brkrCommUom.WriteElements(writer, serializationDepth + 1);
                //    writer.WriteEndElement();
                //}
                ////psGroupCodeRef
                //if (CanSerializeChildObjects && financingBank != null)
                //{
                //    writer.WriteStartElement("financingBank");
                //    writer.WriteAttributeString("xsi", "type", ns, typeof(Account).Name);
                //    financingBank.WriteElements(writer, serializationDepth + 1);
                //    writer.WriteEndElement();
                //}
                //accountAgreement
                //costs
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsExportToExcelRequested = false;
            }
        }

        static string[] _pKeys = new string[] { "tradeNum", "orderNum", "itemNum" };
        public override string[] PrimaryKeys
        {
            get
            {
                return _pKeys;// new string[] { "tradeNum", "orderNum", "itemNum" };
            }
        }

        public override object[] PrimaryValues
        {
            get
            {
                return new object[] { tradeNum, orderNum, itemNum };
            }
        }

        static Type[] keyType = new Type[] { typeof(int?), typeof(int?), typeof(int?) };
        public override Type[] PrimaryKeysTypes
        {
            get
            {
                return keyType; // new Type[] { typeof(int?), typeof(int?), typeof(int?) };
            }
        }

        public virtual bool IsSpecValueMandatory(string specCode)
        {
            return false;
        }


        public Formula mainformula
        {
            get
            {
              return  mainFormula();
            }
            set
            {
            }
        }
        public Formula mainFormula()
        {
            if (CLOP.ntest(this.isFormula()))
            {
                return null;
            }

            List<TradeFormula> tfs = tradeFormulas;
			//Added null check by Naveen
            if (tfs != null)
            {
                IEnumerator<TradeFormula> tfEnum = tfs.GetEnumerator();
                TradeFormula tf;
                if (tfEnum != null)
                {
                    while (CLOP.test(tfEnum.MoveNext()) &&
                            CLOP.test(tf = (TradeFormula)tfEnum.Current))
                    {
                        if (CLOP.Equals(tf.fallBackInd, "N"))
                        {
                            return tf.formula;
                        }
                    }
                }
            }

            return null;
        }

        public Formula riskformula
        {
            get
            {
                return riskFormula();
            }
            set
            {

            }
        }

        public Formula riskFormula()
        {
            if (!this.isFormula())
                return null;
            else
            {
                List<TradeFormula> tfs = tradeFormulas;
                if (tfs != null)
                {
                    foreach (TradeFormula tf in tfs)
                    {
                        if (tf.fallBackInd.Equals("R"))
                            return tf.formula;
                    }
                }

            }

            return null;
        }

        public Formula prelimformula
        {
            get
            {
              return  prelimFormula();
            }
            set
            {
            }
        }
        public Formula prelimFormula()
        {
            if (CLOP.ntest(this.isPrelimFormula()))
            {
                return null;
            }

            List<TradeFormula> tfs = tradeFormulas;
			//Added null check by Naveen
            if (tfs != null)
            {
                IEnumerator<TradeFormula> tfEnum = tfs.GetEnumerator();
                TradeFormula tf;
                if (tfEnum != null)
                {
                    while (CLOP.test(tfEnum.MoveNext()) &&
                            CLOP.test(tf = (TradeFormula)tfEnum.Current))
                    {
                        if (CLOP.Equals(tf.fallBackInd, "Y"))
                        {
                            return tf.formula;
                        }
                    }
                }
            }

            return null;
        }
        public Formula marketformula
        {
            get
            {
                return marketPriceFormula();
            }
            set
            {
            }
        }
        public Formula marketPriceFormula()
        {
            

            List<TradeFormula> tfs = tradeFormulas;
            //Added null check by Naveen
            if (tfs != null)
            {
                IEnumerator<TradeFormula> tfEnum = tfs.GetEnumerator();
                TradeFormula tf;
                if (tfEnum != null)
                {
                    while (CLOP.test(tfEnum.MoveNext()) &&
                            CLOP.test(tf = (TradeFormula)tfEnum.Current))
                    {
                        if (CLOP.Equals(tf.fallBackInd, "M"))
                        {
                            return tf.formula;
                        }
                    }
                }
            }

            return null;
        }
        public bool isFormula()
        {
            if (CLOP.test(CLOP.toBoolean(formulaInd)))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public Uom secondaryUom()
        {
            Uom retUom = null;
            //condition modified by Padma Rao on 15 Jan 2010 to add null check for commodity.secUom as it is throwing error
            //if ((contractQtyUom != null) && (commodity != null) && CLOP.Equals(contractQtyUom.uomType, commodity.secUom.uomType))
            if ((contractQtyUom != null) && (commodity != null) && (commodity.secUom !=null) && CLOP.Equals(contractQtyUom.uomType, commodity.secUom.uomType))
            {
                retUom = commodity.primUom;
            }
            else
            {
                
                if (commodity != null)
                    retUom = commodity.secUom;
            }
            return retUom;
        }

        // Veeru -> 1365216
        public bool noMoreSchedInd()
        {
            bool schedInd = false;
            if (this.schedStatus != null && (this.schedStatus & 2048).Value.Equals(2048))
            {
                schedInd = true;
            }
            return schedInd;
        }

        void TradeOrderPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("tradeOrder." + e.PropertyName);
        }
        
        public TradeItem deepCopy()
        {            
            //TradeItem copy = new TradeItem();                   
            return this;
        }

        #region IComparable Members

        int IComparable.CompareTo(Object tradeItem)
        {
            TradeItem tradeItemOne = this;
            TradeItem tradeItemTwo = (TradeItem)tradeItem;
            int order1 = 0;
            int order2 = 0;
            int item1 = 0;
            int item2 = 0;
            if (tradeItemOne.orderNum != null)
                order1= (int)tradeItemOne.orderNum;
            if (tradeItemTwo.orderNum != null)
             order2 = (int)tradeItemTwo.orderNum;
            if (tradeItemOne.itemNum != null)
             item1 = (int)tradeItemOne.itemNum;
            if (tradeItemTwo.itemNum != null)
                item2 = (int)tradeItemTwo.itemNum;
            if (order1 > order2)
                return 1;
            else if (order1 < order2)
                return -1;
            else
            {
                if (item1 > item2)
                    return 1;
                else
                    return -1;
            }
        }

        #endregion

    }
}