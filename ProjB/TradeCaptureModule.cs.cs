using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using cs.fw.infra.service;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Commands;
using cs.fw.eo.Entity;
using tc.m.tradeCapture.futureTrades;
using cs.fw.ui.controls;
using CSMessageModel.Messages;
using tc.bean.validation;
using com.tc.frameworks.ictseos.eotrade;
using tc.bean.message;
using System.Windows;
using System.Reflection;
using DevComponents.WpfDock;
using System.Windows.Controls;
using tc.m.tradeCapture.controls;
using cs.m.tradeCaptureBusiness;
using cs.fw.eo.fetchspec;
using System.Threading;
using cs.fw.infra;
using tc.m.tradeCapture.Controls;
using tc.m.tradeCapture.listedOptions;
using cs.m.refdataBusiness;
using com.tc.frameworks.ictseos.eocmdtymkt;
using tc.m.tradeCapture.storageAgreement;
using tc.m.tradeCapture.transportAgreement;
using tc.m.tradeCapture.physicalTrades;
using tc.m.tradeCaptureBusiness;
using tc.m.tradeCapture.swapTrades;
using tc.m.tradeCapture.otcOption;
using tc.m.tradeCapture.formulaEdit;
using tc.m.tradeCapture.preferences;
using tc.m.tradeCapture.PhysicalTrades;
using com.tc.frameworks.ictseos.eocommaccount;
using cs.fw.shell.infra.ServiceContracts;
using ReferenceDataBusiness;
using System.ComponentModel;
using com.tc.frameworks.ictseos.eoposition;
using cs.m.referencedata.portfolios;

//using tc.m.tradeCapture.controls.viewModel;

using System.Collections.ObjectModel;
using cs.fw.eo;
using cs.fw.shell.infra;
using cs.fw.ui.regions;
using cs.fw.panel;
using DevExpress.Xpf.Grid;
using tc.m.tradeCapture.controls.scenario;
using cs.m.tradeCapture.allTradeTypes;
using tc.m.tradeCapture.FormulaBodyEditor;
using com.tc.frameworks.ictseos.eotrade.property;

namespace tc.m.tradeCapture
{
    public class TradeCaptureModule : IModule
    {
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;
        private readonly TradeCaptureCommandProxy commandProxy;
        private IRefDataService refDataService;
        private IUserEntityService userEntityService;
        private IPanelService _panelService;
        private static string tradeStatusMessage;

        public DelegateCommand<GenericRecord> OpenNewTradeFormPanelInTabCommand { get; private set; }
        public DelegateCommand<string> OpenTradeFormPanelInTabByTradeNumberCommand { get; private set; }
        public DelegateCommand<string> OpenTradeFormPanelInTabByCPContrNumberCommand { get; private set; }
        public DelegateCommand<string> OpenTradeFormPanelInTabByCINNumberCommand { get; private set; }
        public DelegateCommand<string> NewAsCopyTradeFormPanelInTabByTradeNumberCommand { get; private set; }
        public DelegateCommand<string[]> OpenTradeByTradeNumberCommand { get; private set; }
        public DelegateCommand<string[]> NewAsCopyTradeByTradeNumberCommand { get; private set; }
        public DelegateCommand<TradeOrderOptionsSelected> OpenNewTradePanelCommand { get; private set; }
        public DelegateCommand<string> NewAsCopyStdFldsTradeFormPanelInTabByTradeNumberCommand { get; private set; }
        public DelegateCommand<string[]> NewAsCopyStdFldsTradeByTradeNumberCommand { get; private set; }


        public DelegateCommand<UserControl> Save { get; private set; }
        public DelegateCommand<UserControl> ApplyChangesToDetails { get; private set; }
        public DelegateCommand<Trade> DisplayTotalQtyForAllOrderTypesCommand { get; private set; }

        public DelegateCommand<UserControl> AddItem { get; private set; }
        public DelegateCommand<UserControl> DeleteItem { get; private set; }

        public DelegateCommand<UserControl> AddHedgePhysical { get; private set; }
        public DelegateCommand<UserControl> DeleteHedgePhysical { get; private set; }

        public DelegateCommand<UserControl> AddSpecs { get; private set; }
        public DelegateCommand<UserControl> DeleteSpecs { get; private set; }

        public DelegateCommand<UserControl> AddAllSpecs { get; private set; }
        public DelegateCommand<UserControl> DeleteAllSpecs { get; private set; }

        public DelegateCommand<object> CreateLibFormula { get; private set; }
        public DelegateCommand<object> OpenFormulaByFormulaNumberCommand { get; private set; }

        public DelegateCommand<Dictionary<string, Formula>> OpenModularLineItemComponentsCtlCommand { get; private set; }

        public DelegateCommand<UserControl> SaveFormulaCommand { get; private set; }

        public DelegateCommand<GenericRecord> ShowAllLibraryFormulasCommand { get; private set; }
        public DelegateCommand<GenericRecord> ShowLibraryFormulasForLineItemCommand { get; private set; }

        //Venu-CLC 
        public DelegateCommand<GenericRecord> ShowCLCPanelCommand { get; private set; }

        public DelegateCommand<UserControl> DeleteFormulaCommand { get; private set; }

        public DelegateCommand<PanelCommandParamter> OpenTradeSearchCommand { get; private set; }

        public DelegateCommand<UserControl> SaveScenario { get; private set; }
        public DelegateCommand<UserControl> CreateNewInstanceCommand { get; private set; }
        public DelegateCommand<UserControl> ConfirmAllAllocationForLiveScenarioCommand { get; private set; }
        public DelegateCommand<UserControl> ScheduleScenarioCommand { get; private set; }
        public DelegateCommand<GenericRecord> EditNewLineItemCommand { get; private set; }
        public DelegateCommand<GenericRecord> EditFormulaCompValsCommand { get; private set; }
        public TradeCaptureModule(IRefDataService refDataService, IUserEntityService userEntityService, IUnityContainer container, IRegionManager regionManager, TradeCaptureCommandProxy commandProxy, IWindowService windowService, ISchedulingService SchedulingService, IPanelService panelService)
        {
            this.container = container;
            this.regionManager = regionManager;
            this.commandProxy = commandProxy;
            this.refDataService = refDataService;
            this.userEntityService = userEntityService;
            this._panelService = panelService;

            //this.OpenNewTradeFormPanelInTabCommand = new DelegateCommand<GenericRecord>(OpenNewTradePanel, TradeUtil.CanOpenPanel); //CanOpenPanel as of now true by default
            //this.commandProxy.OpenNewTradeFormPanelInTabCommand.RegisterCommand(this.OpenNewTradeFormPanelInTabCommand);
            this.AddSpecs = new DelegateCommand<UserControl>(OpenAddSpecs, CanAddSpecs);
            this.commandProxy.AddSpecs.RegisterCommand(this.AddSpecs);
            this.DeleteSpecs = new DelegateCommand<UserControl>(OpenDeleteSpecs, CanAddSpecs);
            this.commandProxy.DeleteSpecs.RegisterCommand(this.DeleteSpecs);

            this.AddAllSpecs = new DelegateCommand<UserControl>(OpenAddAllSpecs, CanAddSpecs);
            this.commandProxy.AddAllSpecs.RegisterCommand(this.AddAllSpecs);
            this.DeleteAllSpecs = new DelegateCommand<UserControl>(OpenDeleteAllSpecs, CanAddSpecs);
            this.commandProxy.DeleteAllSpecs.RegisterCommand(this.DeleteAllSpecs);


            this.OpenTradeFormPanelInTabByTradeNumberCommand = new DelegateCommand<string>(OpenTradePanelByTradeNum, TradeUtil.CanOpenPanel); //CanOpenPanel as of now true by default
            this.commandProxy.OpenTradeFormPanelInTabByTradeNumberCommand.RegisterCommand(this.OpenTradeFormPanelInTabByTradeNumberCommand);

            this.OpenTradeFormPanelInTabByCPContrNumberCommand = new DelegateCommand<string>(OpenTradePanelByCPContrNum, TradeUtil.CanOpenPanel); //CanOpenPanel as of now true by default
            this.commandProxy.OpenTradeFormPanelInTabByCPContrNumberCommand.RegisterCommand(this.OpenTradeFormPanelInTabByCPContrNumberCommand);

            this.OpenTradeFormPanelInTabByCINNumberCommand = new DelegateCommand<string>(OpenTradePanelByCINNum, TradeUtil.CanOpenPanel); //CanOpenPanel as of now true by default
            this.commandProxy.OpenTradeFormPanelInTabByCINNumberCommand.RegisterCommand(this.OpenTradeFormPanelInTabByCINNumberCommand);

            this.NewAsCopyTradeFormPanelInTabByTradeNumberCommand = new DelegateCommand<string>(NewAsCopyTradePanelByTradeNum, TradeUtil.CanOpenPanel); //CanOpenPanel as of now true by default
            this.commandProxy.NewAsCopyTradeFormPanelInTabByTradeNumberCommand.RegisterCommand(this.NewAsCopyTradeFormPanelInTabByTradeNumberCommand);

            this.NewAsCopyStdFldsTradeFormPanelInTabByTradeNumberCommand = new DelegateCommand<string>(NewAsCopyStdFldsTradePanelByTradeNum, TradeUtil.CanOpenPanel);
            this.commandProxy.NewAsCopyStdFldsTradeFormPanelInTabByTradeNumberCommand.RegisterCommand(this.NewAsCopyStdFldsTradeFormPanelInTabByTradeNumberCommand);
            this.OpenNewTradePanelCommand = new DelegateCommand<TradeOrderOptionsSelected>(OpenTradePanel, TradeUtil.CanOpenPanel);
            this.commandProxy.OpenNewTradePanelCommand.RegisterCommand(this.OpenNewTradePanelCommand);

            this.Save = new DelegateCommand<UserControl>(SaveTradeData, TradeUtil.CanOpenPanel);
            this.commandProxy.Save.RegisterCommand(this.Save);

            this.ApplyChangesToDetails = new DelegateCommand<UserControl>(ApplyChangesToDetail, TradeUtil.CanOpenPanel);
            this.commandProxy.ApplyChangesToDetails.RegisterCommand(this.ApplyChangesToDetails);

            this.DisplayTotalQtyForAllOrderTypesCommand = new DelegateCommand<Trade>(DisplayTotalQtyForAllOrderTypes, TradeUtil.CanOpenPanel);
            this.commandProxy.DisplayTotalQtyForAllOrderTypesCommand.RegisterCommand(this.DisplayTotalQtyForAllOrderTypesCommand);


            this.AddItem = new DelegateCommand<UserControl>(AddItemToTrade, TradeUtil.CanOpenPanel);
            this.commandProxy.AddItem.RegisterCommand(this.AddItem);

            this.DeleteItem = new DelegateCommand<UserControl>(DeleteItemFromTrade, TradeUtil.CanOpenPanel);
            this.commandProxy.DeleteItem.RegisterCommand(this.DeleteItem);

            this.OpenTradeByTradeNumberCommand = new DelegateCommand<string[]>(OpenTradePanelByTradeNumber, TradeUtil.CanOpenPanel);
            this.commandProxy.OpenTradeByTradeNumberCommand.RegisterCommand(this.OpenTradeByTradeNumberCommand);


            this.NewAsCopyTradeByTradeNumberCommand = new DelegateCommand<string[]>(NewAsCopyTradePanelByTradeNumber, TradeUtil.CanOpenPanel);
            this.commandProxy.NewAsCopyTradeByTradeNumberCommand.RegisterCommand(this.NewAsCopyTradeByTradeNumberCommand);
            this.NewAsCopyStdFldsTradeByTradeNumberCommand = new DelegateCommand<string[]>(NewAsCopyStdFldsTradePanelByTradeNumber, TradeUtil.CanOpenPanel);
            this.commandProxy.NewAsCopyStdFldsTradeByTradeNumberCommand.RegisterCommand(this.NewAsCopyStdFldsTradeByTradeNumberCommand);

            this.AddHedgePhysical = new DelegateCommand<UserControl>(OpenAddHedgePhysical, TradeUtil.CanOpenPanel);
            this.commandProxy.AddHedgePhysical.RegisterCommand(this.AddHedgePhysical);
            this.DeleteHedgePhysical = new DelegateCommand<UserControl>(OpenDeleteHedgePhysical, TradeUtil.CanOpenPanel);
            this.commandProxy.DeleteHedgePhysical.RegisterCommand(this.DeleteHedgePhysical);

            this.CreateLibFormula = new DelegateCommand<object>(OpenCreateLibFormula, TradeUtil.CanOpenPanel);
            this.commandProxy.CreateLibFormula.RegisterCommand(this.CreateLibFormula);

            this.OpenFormulaByFormulaNumberCommand = new DelegateCommand<object>(OpenFormulaPanelByFormulaNum, TradeUtil.CanOpenPanel); //CanOpenPanel as of now true by default
            this.commandProxy.OpenFormulaByFormulaNumberCommand.RegisterCommand(this.OpenFormulaByFormulaNumberCommand);

            this.OpenModularLineItemComponentsCtlCommand = new DelegateCommand<Dictionary<string, Formula>>(OpenModularLineItemComponentsCtl, TradeUtil.CanOpenPanel); //CanOpenPanel as of now true by default
            this.commandProxy.OpenModularLineItemComponentsCtlCommand.RegisterCommand(this.OpenModularLineItemComponentsCtlCommand);

            this.SaveFormulaCommand = new DelegateCommand<UserControl>(SaveLibraryFormulaData, TradeUtil.CanOpenPanel);
            this.commandProxy.SaveFormulaCommand.RegisterCommand(this.SaveFormulaCommand);

            this.ShowAllLibraryFormulasCommand = new DelegateCommand<GenericRecord>(ShowAllLibraryFormulasData, TradeUtil.CanOpenPanel);
            this.commandProxy.ShowAllLibraryFormulasCommand.RegisterCommand(this.ShowAllLibraryFormulasCommand);

            this.ShowLibraryFormulasForLineItemCommand = new DelegateCommand<GenericRecord>(ShowLibraryFormulasForLineItemData);
            this.commandProxy.ShowLibraryFormulasForLineItemCommand.RegisterCommand(this.ShowLibraryFormulasForLineItemCommand);

            //Venu-CLC 
            this.ShowCLCPanelCommand = new DelegateCommand<GenericRecord>(ShowCLCPanelData, TradeUtil.CanOpenPanel);
            this.commandProxy.ShowCLCPanelCommand.RegisterCommand(this.ShowCLCPanelCommand);

            this.DeleteFormulaCommand = new DelegateCommand<UserControl>(DeleteLibraryFormulaData, TradeUtil.CanOpenPanel);
            this.commandProxy.DeleteFormulaCommand.RegisterCommand(this.DeleteFormulaCommand);

            this.OpenTradeSearchCommand = new DelegateCommand<PanelCommandParamter>(OpenTradeSearchPanel, TradeUtil.CanOpenPanel);
            this.commandProxy.OpenTradeSearchCommand.RegisterCommand(this.OpenTradeSearchCommand);

            this.SaveScenario = new DelegateCommand<UserControl>(SaveScenarioData, TradeUtil.CanOpenPanel);
            this.commandProxy.SaveScenario.RegisterCommand(this.SaveScenario);

            this.CreateNewInstanceCommand = new DelegateCommand<UserControl>(CreateLiveScenarioForScenario, TradeUtil.CanOpenPanel);
            this.commandProxy.CreateNewInstanceCommand.RegisterCommand(this.CreateNewInstanceCommand);

            this.ConfirmAllAllocationForLiveScenarioCommand = new DelegateCommand<UserControl>(ConfirmAllAllocationForLiveScenario, TradeUtil.CanOpenPanel);
            this.commandProxy.ConfirmAllAllocationForLiveScenarioCommand.RegisterCommand(this.ConfirmAllAllocationForLiveScenarioCommand);

            this.ScheduleScenarioCommand = new DelegateCommand<UserControl>(ScheduleScenarioData, TradeUtil.CanOpenPanel);
            this.commandProxy.ScheduleScenarioCommand.RegisterCommand(this.ScheduleScenarioCommand);

            this.EditNewLineItemCommand = new DelegateCommand<GenericRecord>(EditNewLineItem, TradeUtil.CanOpenPanel);
            this.commandProxy.EditNewLineItemCommand.RegisterCommand(this.EditNewLineItemCommand);

            this.EditFormulaCompValsCommand = new DelegateCommand<GenericRecord>(EditFormulaCompVals, TradeUtil.CanOpenPanel);
            this.commandProxy.EditFormulaCompValsCommand.RegisterCommand(this.EditFormulaCompValsCommand);



        }

        public void SaveScenarioData(UserControl userCtl)
        {
            ValidateAndSaveScenarioData((userCtl as ScenarioSetupCtl).dataStoreService as IDataStoreService, null);
        }

        public void CreateLiveScenarioForScenario(UserControl userCtl)
        {
            if (userCtl is ScenarioInstanceCtl)
                ValidateAndCreateLiveScenario((userCtl as ScenarioInstanceCtl).dataStoreService as IDataStoreService, (userCtl as ScenarioInstanceCtl).scenarioToInstantiate, userCtl);
        }

        private void ValidateAndCreateLiveScenario(IDataStoreService dataService, GenericRecord genericRecord, UserControl userCtl)
        {
            //TODO need to add validation here.
            ViewState viewState = dataService.CreateViewState();

            List<GenericMessage> listOfMessages = new List<GenericMessage>();
            CreateLiveScenario liveScenario = new CreateLiveScenario();
            if (genericRecord != null && genericRecord is Scenario)
            {
                liveScenario.scenarioRecordKey = (genericRecord).RecordKey;
            }
            listOfMessages.Add(liveScenario);
            ProcessSaveCreateLiveScenarioChanges(listOfMessages, viewState, dataService, genericRecord, userCtl);

        }

        public void ConfirmAllAllocationForLiveScenario(UserControl userCtl)
        {
            if (userCtl is LiveScenariosCtl && (userCtl as LiveScenariosCtl).liveScenarioDetailsGrid != null && (userCtl as LiveScenariosCtl).liveScenarioDetailsGrid.SelectedItem != null)
            {
                ValidateAndConfirmAllAllocation((userCtl as LiveScenariosCtl).dataStoreService as IDataStoreService, (userCtl as LiveScenariosCtl).liveScenarioDetailsGrid.SelectedItem as LiveScenario);
            }
        }

        private void ValidateAndConfirmAllAllocation(IDataStoreService dataService, GenericRecord genericRecord)
        {
            //TODO need to add validation here.
            ViewState viewState = dataService.CreateViewState();
            List<GenericMessage> listOfMessages = new List<GenericMessage>();
            ConfirmAllAllocation confirmAllAllocation = new ConfirmAllAllocation();
            if (genericRecord != null && genericRecord is LiveScenario)
            {
                confirmAllAllocation.liveScenarioRecordKey = (genericRecord).RecordKey;
                confirmAllAllocation.oid = (genericRecord as LiveScenario).oid;
            }
            listOfMessages.Add(confirmAllAllocation);
            ProcessConfirmAllAllocation(listOfMessages, viewState, dataService, genericRecord);
        }

        private void ProcessConfirmAllAllocation(List<GenericMessage> listOfMessages, ViewState viewState, IDataStoreService dataService, GenericRecord genericRecord)
        {
            string jBossMachineName = null;
            string failurMsg = string.Empty;
            StringBuilder sb = new StringBuilder();
            string successMsg = string.Empty;

            CompositeMessage message = new CompositeMessage();
            message.Messages = listOfMessages;
            IMessageProcessorService processService = container.Resolve<IMessageProcessorService>();
            CompositeResponseMessage response = null;
            response = processService.ProcessConfirmAllAllocation(message, viewState, dataService);

            foreach (ResponseMessage resposeMsg in response.responseMessages)
            {
                if (resposeMsg.statusMessage != null && resposeMsg.statusMessage.Contains("Failure"))
                {
                    jBossMachineName = resposeMsg.machineName;
                    failurMsg = "Records Not Saved" + System.Environment.NewLine;
                    sb.Append(resposeMsg.statusMessage).Append(Environment.NewLine);

                    if (resposeMsg.HasStaleObjects())
                    {
                        failurMsg += buildStaleObjectsErrorMessage(resposeMsg.staleObjectsRecordKey);
                        //single line of code commented and the next line aded by Padma Rao on 19 Apr 2012 based on issue 1344189.
                        /*
                            * The message is modified based on teh mail sent by Gwen on 18 Apr 2012 
                            */
                        //failurMsg += "Do you want to Refresh the objects ?";
                        failurMsg += "Would you like to refresh the objects and Save?";
                        MessageBoxResult result = MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            foreach (string recordKey in resposeMsg.staleObjectsRecordKey)
                            {
                                EntityRecord localGenericRecord = dataService.GetGenericObject(recordKey) as EntityRecord;
                                //condition added by Padma Rao oN 24 Sep 2012 based on issue 1356105
                                /*
                                    * condition added based on the rolling logs for gwoody in cs3 env on sep 12 2012
                                    */
                                if (localGenericRecord != null)
                                {
                                    //ServiceUtil.FetchStaleObject<EntityRecord>(localGenericRecord);
                                    ServiceUtil.FetchStaleObject<EntityRecord>(localGenericRecord, dataService);
                                    int? transId = null;

                                    try
                                    {
                                        transId = Convert.ToInt32(localGenericRecord.GetType().GetProperty("transId").GetValue(localGenericRecord, null));

                                    }
                                    catch (Exception e)
                                    {
                                    }

                                    string logmsg = "StaleObject -> " + recordKey + " New TransId : " + transId;
                                }
                            }
                            ValidateAndConfirmAllAllocation(dataService, genericRecord);
                            return;
                        }
                    }
                    else
                    {
                        if (resposeMsg.validationResults != null)
                        {
                            failurMsg = string.Empty;
                            //looping through all the validation results and adding to the display message
                            foreach (tc.bean.validation.ValidationResult vr in response.responseMessages[0].validationResults.results)
                            {
                                failurMsg += vr.ToString() + System.Environment.NewLine;
                            }

                            MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.OK);
                        }
                    }
                }//end of failure block
                else if (resposeMsg.statusMessage != null && resposeMsg.statusMessage.Contains("Success"))
                {
                    if (resposeMsg.validationResults.results != null)
                    {
                        foreach (tc.bean.validation.ValidationResult vr in resposeMsg.validationResults.results)
                        {
                            successMsg += vr.ToString() + System.Environment.NewLine;
                        }
                        string msg = null;
                        string ScenariosList = null;

                        if (resposeMsg.affectedObjectsRecordKey != null && resposeMsg.affectedObjectsRecordKey.Count > 0)
                        {
                            foreach (String recKey in resposeMsg.affectedObjectsRecordKey)
                            {
                                if (recKey.Contains("LiveScenario:"))
                                {
                                    if (ScenariosList == null)
                                        ScenariosList = recKey.Substring(recKey.IndexOf('=') + 1).Replace(';', ' ');
                                    else
                                        ScenariosList += "/ " + recKey.Substring(recKey.IndexOf('=') + 1).Replace(';', ' ');
                                }
                                GenericRecord gr = dataService.GetGenericObject(recKey);
                                if (gr != null)
                                    gr.AcceptChanges();
                            }
                            msg = "LiveScenario# " + ScenariosList + " saved successfully";
                        }

                        if (msg == null && successMsg != null)
                            msg = successMsg;
                        MessageBox.Show(msg, "Trade Capture", MessageBoxButton.OK);

                        ScenarioUtil.scenariosLst = null;
                        ScenarioUtil.FetchAllScenarios(dataService);
                        dataService.ResetAllChanges();
                    }
                }
            }

        }


        public void ScheduleScenarioData(UserControl userCtl)
        {
            //ScenarioInstanceCtl
            if (userCtl is ScenarioInstanceCtl && (userCtl as ScenarioInstanceCtl).scenarioToInstantiate != null)
            {
                ValidateAndScheduleScenarioData((userCtl as ScenarioInstanceCtl).dataStoreService as IDataStoreService, (userCtl as ScenarioInstanceCtl).scenarioToInstantiate as Scenario, userCtl as ScenarioInstanceCtl);
            }
        }

        private void ValidateAndScheduleScenarioData(IDataStoreService dataService, GenericRecord genericRecord, ScenarioInstanceCtl scenInstanceCtl)
        {
            //TODO need to add validation here.
            ViewState viewState = dataService.CreateViewState();
            List<GenericMessage> listOfMessages = new List<GenericMessage>();
            ScheduleScenario scheduleScenario = new ScheduleScenario();
            if (genericRecord != null && genericRecord is Scenario)
            {
                scheduleScenario.scenarioRecordKey = (genericRecord).RecordKey;
                scheduleScenario.oid = (genericRecord as Scenario).oid;
                if (scenInstanceCtl.txtScheduleName != null && scenInstanceCtl.txtScheduleName.Text != null && !scenInstanceCtl.txtScheduleName.Text.Equals(string.Empty))
                    scheduleScenario.sceSchedName = scenInstanceCtl.txtScheduleName.Text.ToString();
                if (scenInstanceCtl.ddScheduleDate != null && scenInstanceCtl.ddScheduleDate.EditValue != null)
                    scheduleScenario.sceSchedDate = String.Format("{0:dd-MMM-yy}", scenInstanceCtl.ddScheduleDate.EditValue);// scenInstanceCtl.ddScheduleDate.EditValue.ToString();
                if (scenInstanceCtl.spnEditTimeHr != null && scenInstanceCtl.spnEditTimeHr.EditValue != null && scenInstanceCtl.spnEditTimeMin != null && scenInstanceCtl.spnEditTimeMin.EditValue != null)
                    scheduleScenario.sceSchedTime = scenInstanceCtl.spnEditTimeHr.EditValue.ToString() + ":" + scenInstanceCtl.spnEditTimeMin.EditValue.ToString();
            }
            listOfMessages.Add(scheduleScenario);
            ProcessScheduleScenario(listOfMessages, viewState, dataService, genericRecord, scenInstanceCtl);
        }

        private void ProcessScheduleScenario(List<GenericMessage> listOfMessages, ViewState viewState, IDataStoreService dataService, GenericRecord genericRecord, ScenarioInstanceCtl scenInstanceCtl)
        {
            string jBossMachineName = null;
            string failurMsg = string.Empty;
            StringBuilder sb = new StringBuilder();
            string successMsg = string.Empty;

            CompositeMessage message = new CompositeMessage();
            message.Messages = listOfMessages;
            IMessageProcessorService processService = container.Resolve<IMessageProcessorService>();
            CompositeResponseMessage response = null;
            response = processService.ProcessScheduleScenario(message, viewState, dataService);

            foreach (ResponseMessage resposeMsg in response.responseMessages)
            {
                if (resposeMsg.statusMessage != null && resposeMsg.statusMessage.Contains("Failure"))
                {
                    jBossMachineName = resposeMsg.machineName;
                    failurMsg = "Records Not Saved" + System.Environment.NewLine;
                    sb.Append(resposeMsg.statusMessage).Append(Environment.NewLine);

                    if (resposeMsg.HasStaleObjects())
                    {
                        failurMsg += buildStaleObjectsErrorMessage(resposeMsg.staleObjectsRecordKey);
                        //single line of code commented and the next line aded by Padma Rao on 19 Apr 2012 based on issue 1344189.
                        /*
                            * The message is modified based on teh mail sent by Gwen on 18 Apr 2012 
                            */
                        //failurMsg += "Do you want to Refresh the objects ?";
                        failurMsg += "Would you like to refresh the objects and Save?";
                        MessageBoxResult result = MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            foreach (string recordKey in resposeMsg.staleObjectsRecordKey)
                            {
                                EntityRecord localGenericRecord = dataService.GetGenericObject(recordKey) as EntityRecord;
                                //condition added by Padma Rao oN 24 Sep 2012 based on issue 1356105
                                /*
                                    * condition added based on the rolling logs for gwoody in cs3 env on sep 12 2012
                                    */
                                if (localGenericRecord != null)
                                {
                                    //ServiceUtil.FetchStaleObject<EntityRecord>(localGenericRecord);
                                    ServiceUtil.FetchStaleObject<EntityRecord>(localGenericRecord, dataService);
                                    int? transId = null;

                                    try
                                    {
                                        transId = Convert.ToInt32(localGenericRecord.GetType().GetProperty("transId").GetValue(localGenericRecord, null));

                                    }
                                    catch (Exception e)
                                    {
                                    }

                                    string logmsg = "StaleObject -> " + recordKey + " New TransId : " + transId;
                                }
                            }
                            ValidateAndScheduleScenarioData(dataService, genericRecord, scenInstanceCtl);
                            return;
                        }
                    }
                    else
                    {
                        if (resposeMsg.validationResults != null)
                        {
                            failurMsg = string.Empty;
                            //looping through all the validation results and adding to the display message
                            foreach (tc.bean.validation.ValidationResult vr in response.responseMessages[0].validationResults.results)
                            {
                                failurMsg += vr.ToString() + System.Environment.NewLine;
                            }

                            MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.OK);
                        }
                    }
                }//end of failure block
                else if (resposeMsg.statusMessage != null && resposeMsg.statusMessage.Contains("Success"))
                {
                    if (resposeMsg.validationResults.results != null)
                    {
                        foreach (tc.bean.validation.ValidationResult vr in resposeMsg.validationResults.results)
                        {
                            successMsg += vr.ToString() + System.Environment.NewLine;
                        }
                        string msg = null;
                        string ScenariosList = null;

                        if (resposeMsg.affectedObjectsRecordKey != null && resposeMsg.affectedObjectsRecordKey.Count > 0)
                        {
                            foreach (String recKey in resposeMsg.affectedObjectsRecordKey)
                            {
                                if (recKey.Contains("Scenario:"))
                                {
                                    if (ScenariosList == null)
                                        ScenariosList = recKey.Substring(recKey.IndexOf('=') + 1).Replace(';', ' ');
                                    else
                                        ScenariosList += "/ " + recKey.Substring(recKey.IndexOf('=') + 1).Replace(';', ' ');
                                }
                                GenericRecord gr = dataService.GetGenericObject(recKey);
                                if (gr != null)
                                    gr.AcceptChanges();
                            }
                            msg = "Scenario# " + ScenariosList + " saved successfully";
                        }

                        if (msg == null && successMsg != null)
                            msg = successMsg;
                        MessageBox.Show(msg, "Trade Capture", MessageBoxButton.OK);

                        ScenarioUtil.scenariosLst = null;
                        ScenarioUtil.FetchAllScenarios(dataService);
                        dataService.ResetAllChanges();
                    }
                }
            }

        }

        public void OpenTradeSearchPanel(PanelCommandParamter param)
        {
            ////T# 21106387
            ////if (_panelService != null && _panelService.DockSites != null && _panelService.DockSites.Count > 0)
            ////{
            ////    foreach (DockWindow dwindow in (_panelService.DockSites[0] as DockSite).GetDockWindows())
            ////    {
            ////        if (dwindow.Header.ToString().Equals("All Trades 1") || dwindow.Header.ToString().Equals("Trade Search 1"))
            ////        {
            ////            dwindow.IsSelected = true;
            ////            return;
            ////        }
            ////    }
            ////}
            OpenPhysicalTradePanel(param);


        }
        cs.fw.infra.view.querybuilder.RecordInspector recordInspector = null;
        CSFetchSpecificationPModel recentSpec = null;
        private IView OpenPhysicalTradePanel(PanelCommandParamter panelCommandParameter)
        {
            IWindowService windowSvc = container.Resolve<IWindowService>();
            IUserEntityView view = new tc.m.tradeCapture.tradeSearch.TradeSearchView(refDataService, new cs.m.common.allTradeTypes.AllTradeTypesFetchSpecifications()[panelCommandParameter.Specification] as CSFetchSpecification, panelCommandParameter.Specification, true);
            if (view != null)
            {
                windowSvc.AddPanelToWindow(container, null, "TradeSearch", view);
            }
            return view;
        }
        private void CloseCostPopup()
        {
            Window window = (recordInspector.Parent as DockPanel).Parent as Window;
            window.Close();
        }
        void btnNewAsCopy_Click(object sender, RoutedEventArgs e)
        {
            if (recordInspector != null && recordInspector.TableCtl is TableCtl)
            {
                string tradeNum;
                if ((recordInspector.TableCtl as TableCtl).SelectedItems != null && (recordInspector.TableCtl as TableCtl).SelectedItems.Count > 0)
                {
                    tradeNum = ((recordInspector.TableCtl as TableCtl).SelectedItems[0] as TradeItem).tradeNum.ToString();
                    String[] tradeNumStrings = tradeNum.Trim().Split(',');
                    CloseCostPopup();
                    this.NewAsCopyTradeByTradeNumberCommand.Execute(tradeNumStrings);

                }
            }
        }

        public void OpenModularLineItemComponentsCtl(Dictionary<string, Formula> formulaDict)
        {
            //if (formula != null && formula is Formula)
            if (formulaDict != null && formulaDict.Count() == 2)
            {
                Formula aLibFormula = formulaDict["selectedLibFormula"];
                Formula aTradeFormula = formulaDict["selectedTradeFormula"];
                if (aLibFormula != null && aTradeFormula != null)
                {
                    //IDataStore dataStoreService = DataStoreServiceHelper.createNewDataStoreService();
                    //cs.fw.infra.service.IDataStoreService dataStore = (cs.fw.infra.service.IDataStoreService)dataStoreService;
                    //string mergeType = GenericRecord.UserChangesMergeOverwriteUserChanges;
                    //IMybatisDataAccessService MybatisDataAccessService = container.Resolve<IMybatisDataAccessService>();
                    //CSFetchSpecification fetchSpecification = new CSFetchSpecification("Formula", "Tradelayout", SpecificationTypes.System);
                    //CSKeyValueQualifier qualifier = new CSKeyValueQualifier("f.formula_num", CSQualifierOperatorSelectors.QualifierOperatorEqual, aFormula.formulaNum);
                    //fetchSpecification.Qualifier = qualifier;
                    //List<Formula> formulas = MybatisDataAccessService.FetchForQuery<Formula>(fetchSpecification, true, true, cs.fw.eo.EOUtil.GetResultMapName("Formula"), true, null, true, dataStore);
                    //if (formulas == null || formulas.Count == 0)
                    //{
                    //    MessageBox.Show("Formula # " + aFormula.formulaNum + " not found!", null, MessageBoxButton.OK, MessageBoxImage.Information);
                    //    return;
                    //}
                    //else
                    //{
                    //    formulas[0].dataStoreService = dataStore;
                    //    aFormula = formulas[0];

                    //Dictionary<string, FormulaComponent> formulaCompDict = aLibFormula.getFCObjectsFromSummaryTextTokens();

                    if (aLibFormula.formulaBodies != null && aLibFormula.formulaBodies.Count > 0)
                    {
                        FormulaBody formulaBody = aLibFormula.formulaBodies.Where(fb => fb.formulaBodyType.Equals("P")).FirstOrDefault();
                        if (formulaBody != null)
                        {
                            formulaBody = formulaBody.deepCopy();////deepCopyObject only we are using in this case
                            //if (aTradeFormula != null && formulaBody.fbModularInfo !=null && formulaBody.fbModularInfo.riskTradingPeriod !=null && aTradeFormula.tradeFormula[0].tradeItem.trade.contrDate > formulaBody.fbModularInfo.riskTradingPeriod.lastDelDate)
                            if (aTradeFormula.tradeFormula != null && aTradeFormula.tradeFormula.Count > 0 && aTradeFormula.tradeFormula[0].tradeItem != null)
                            {
                                formulaBody.fbModularInfo.riskTradingPeriod = null;
                                formulaBody.fbModularInfo.riskTradingPrd = null;
                                if (formulaBody.fbModularInfo != null && formulaBody.fbModularInfo.basisCommodity != null && formulaBody.fbModularInfo.basisCommodity.IsTradeable)
                                {
                                    TradingPeriod trpd = null;
                                    if (formulaBody.fbModularInfo.basisCmdtyCode != null && formulaBody.fbModularInfo.riskMktCode != null && aTradeFormula.tradeFormula[0].tradeItem.tradingPrd != null)
                                        trpd = TradingPeriod.GetTradingPeriod(formulaBody.fbModularInfo.basisCmdtyCode, formulaBody.fbModularInfo.riskMktCode, aTradeFormula.tradeFormula[0].tradeItem.tradingPrd);
                                    if (trpd != null)
                                    {
                                        formulaBody.fbModularInfo.riskTradingPeriod = trpd;
                                        formulaBody.fbModularInfo.riskTradingPrd = trpd.tradingPrd;
                                    }
                                }
                            }
                            formulaBody.formula = aTradeFormula;
                            //Dictionary<string, FormulaComponent> formulaCompDict = aLibFormula.getFCObjectsFromSummaryTextTokens();
                            //Dictionary<string, FormulaComponent> formulaCompDict = formulaBody.getFCObjectsFromSummaryTextTokens();
                            if (formulaBody.formulaComponents != null && formulaBody.formulaComponents.Count() > 0)
                            {
                                DataStoreService dataStoreService = (DataStoreService)formulaBody.dataStoreService;
                                ModularLineItemComponentsCtl lineItemsCtl = new ModularLineItemComponentsCtl();
                                lineItemsCtl.DataContext = formulaBody.fbModularInfo;
                                lineItemsCtl.selectedFormulaDict = formulaDict;
                                // Added code for removing the Event price terms on formula for storage trade. Added by devender M
                                if (aTradeFormula.tradeFormula != null && aTradeFormula.tradeFormula.Count > 0 && aTradeFormula.tradeFormula[0].tradeItem.isTypeInventory)
                                {
                                    if (formulaBody._fbEventPriceTerm != null && formulaBody._fbEventPriceTerm.Count > 0)
                                    {
                                        foreach (FbEventPriceTerm fbept in formulaBody._fbEventPriceTerm)
                                        {
                                            if (dataStoreService != null)
                                                (dataStoreService).RemoveObjectAndRelations(fbept);
                                        }
                                    }
                                    formulaBody._fbEventPriceTerm = null;
                                    formulaBody.rangeType = null;
                                }
                                lineItemsCtl.selectedLibFormulaBodyDeepCopy = formulaBody;//deepCopyObject
                                addFCCtrlsToModulatLineItemCtrl(formulaBody.formulaComponents, lineItemsCtl, aTradeFormula);
                                //foreach (string key in formulaCompDict.Keys)
                                //{
                                //    FormulaComponent fComp = formulaCompDict[key];
                                //    //fComp.formulaCompNum = 0;
                                //    if (fComp.isTypeWeight())
                                //    {
                                //        WeightedComponent weighComp = new WeightedComponent();
                                //        weighComp.DataContext = fComp;
                                //        //weighComp.txtTitle.Text = fComp.formulaCompLabel;
                                //        //weighComp.txtTitle.Text = fComp.formulaCompName;
                                //        //weighComp.dFormulaUom.lookupEdit.SelectedItem = fComp.uom;
                                //        lineItemsCtl.stkComponents.Children.Add(weighComp);
                                //    }
                                //    else if (fComp.formulaCompType.Equals("G"))
                                //    {
                                //        QuoteComponent quoteComp = new QuoteComponent();
                                //        quoteComp.DataContext = fComp;
                                //        //quoteComp.txtTitle.Text = fComp.formulaCompLabel;
                                //        //quoteComp.txtTitle.Text = fComp.formulaCompName;
                                //        //quoteComp.dFormulaUom.lookupEdit.SelectedItem = fComp.uom;
                                //        //quoteComp.quoteSelector.textEditor.Text = fComp.quoteDesc();
                                //        lineItemsCtl.stkComponents.Children.Add(quoteComp);
                                //    }
                                //    else
                                //    {
                                //        EstimatesComponent estimateComp = new EstimatesComponent();
                                //        estimateComp.DataContext = fComp;
                                //        //estimateComp.txtTitle.Text = fComp.formulaCompLabel;
                                //        //estimateComp.txtTitle.Text = fComp.formulaCompName;
                                //        //estimateComp.dFormulaUom.lookupEdit.SelectedItem = fComp.uom;
                                //        //estimateComp.dFormulaCurrency.lookupEdit.SelectedItem = fComp.currency;
                                //        lineItemsCtl.stkComponents.Children.Add(estimateComp);
                                //    }
                                //}
                                //TODO : Display Form View of  FC
                                IPopupService popupService = container.Resolve<IPopupService>();
                                popupService.AddPopup("Line item components", lineItemsCtl, false, false);
                            }
                            else // FB shoud have FC's hence below else block never executed
                                TradeCaptureUtil.AddNewPriceLineItem(aLibFormula, aTradeFormula, formulaBody);
                        }
                    }
                }

            }
        }
        public void EditNewLineItem(GenericRecord record)
        {
            FormulaBody formulaBody = record as FormulaBody;
            Dictionary<string, Formula> formulaDict = new Dictionary<string, Formula>();
            if (formulaBody != null)//&& formulaBody.formulaComponents != null && formulaBody.formulaComponents.Count > 0)
            {
                if (formulaBody.fbModularInfo != null && formulaBody.fbModularInfo.basisCommodity != null && !formulaBody.fbModularInfo.basisCommodity.IsTradeable && (formulaBody.formulaComponents == null || formulaBody.formulaComponents.Count == 0))
                {
                    MessageBox.Show("Selected item has non tradable commodity and there is no formula components to edit.", "TradeCapture", MessageBoxButton.OK);
                    return;
                }
                else
                {
                    ModularLineItemComponentsCtl lineItemsCtl = new ModularLineItemComponentsCtl();
                    //lineItemsCtl.btnOk.Visibility = Visibility.Hidden;
                    //lineItemsCtl.btnCancel.Content = "Ok";
                    lineItemsCtl.btnCancel.Visibility = Visibility.Hidden;
                    lineItemsCtl.DataContext = formulaBody.fbModularInfo;
                    formulaDict["selectedLibFormula"] = formulaBody.formula;
                    formulaDict["selectedTradeFormula"] = formulaBody.formula;
                    lineItemsCtl.selectedFormulaDict = formulaDict;
                    lineItemsCtl.selectedLibFormulaBodyDeepCopy = formulaBody;//deepCopyObject
                    //editFCCtrlsToModulatLineItemCtrl(formulaBody.formulaComponents, lineItemsCtl);
                    addFCCtrlsToModulatLineItemCtrl(formulaBody.formulaComponents, lineItemsCtl, formulaBody.formula);
                    //TODO : Display Form View of  FC
                    IPopupService popupService = container.Resolve<IPopupService>();
                    popupService.AddPopup("Edit line item", lineItemsCtl, false, false);
                }
            }
        }

        public void EditFormulaCompVals(GenericRecord record)
        {
            FormulaBody formulaBody = record as FormulaBody;
            if (formulaBody != null && formulaBody.formula != null)
            {
                EditFormulaComponentsCtrl editFormulaCompsCtl = new EditFormulaComponentsCtrl();
                editFormulaCompsCtl.DataContext = formulaBody.formula;
                editFCCtrlsToEditFormulaComponentsCtrl(formulaBody.formula, formulaBody.formula.getAllFormulaComponetsAsList(), editFormulaCompsCtl);
                //editSpecFCCtrlsToEditFormulaComponentsCtrl(formulaBody.formula.specCompsList, editFormulaCompsCtl);
                editSpecFCCtrlsToEditFormulaComponentsCtrlasGrid(formulaBody.formula.tradeItemspecList, editFormulaCompsCtl);
                IPopupService popupService = container.Resolve<IPopupService>();
                popupService.AddPopup("Edit Formula Components", editFormulaCompsCtl, false, false);
            }
        }

        private void editSpecFCCtrlsToEditFormulaComponentsCtrlasGrid(List<TradeItemSpec> tradeItemSpecList, EditFormulaComponentsCtrl editFormulaCompsCtl)
        {
            if (tradeItemSpecList != null && tradeItemSpecList.Count() > 0
                && editFormulaCompsCtl != null)
            {
                StackPanel formulaStackpanel = editFormulaCompsCtl.getStkformulaSpecifications();
                if (formulaStackpanel != null)
                {
                    formulaStackpanel.DataContext = tradeItemSpecList[0].tradeItem;
                    (formulaStackpanel.Children[0] as GridControl).ItemsSource = tradeItemSpecList.Distinct<TradeItemSpec>().OrderBy(o => o.specCode).ToList<TradeItemSpec>();

                }
            }
        }
        //private void editSpecFCCtrlsToEditFormulaComponentsCtrl(List<FormulaComponent> specFormulaComps, EditFormulaComponentsCtrl editFormulaCompsCtl)
        //{
        //    if (specFormulaComps != null && specFormulaComps.Count() > 0
        //        && editFormulaCompsCtl != null)
        //    {
        //        StackPanel formulaStackpanel = editFormulaCompsCtl.getStkformulaSpecifications();
        //        foreach (FormulaComponent fComp in specFormulaComps)
        //        {
        //            FormulaSpecificationsCtrl formulaSpecControl = new FormulaSpecificationsCtrl();
        //            formulaSpecControl.DataContext = fComp.formulaCompSpec;
        //            formulaStackpanel.Children.Add(formulaSpecControl);
        //        }
        //    }
        //}

        private void editFCCtrlsToEditFormulaComponentsCtrl(Formula aSelectedTradeFormula, List<FormulaComponent> formulaComps, EditFormulaComponentsCtrl editFormulaCompsCtl)
        {
            if (formulaComps != null && formulaComps.Count() > 0
                && editFormulaCompsCtl != null)
            {
                StackPanel formulaStackpanel = editFormulaCompsCtl.getStkformulaVariables();
                StackPanel qpOptionalityStackpanel = editFormulaCompsCtl.getStkQPOptionalityTabItem();
                if (qpOptionalityStackpanel != null)
                {
                    qpOptionalityStackpanel.DataContext = aSelectedTradeFormula.modularFormulaBodiesHavingQuoteinpriceQuote;
                    (qpOptionalityStackpanel.Children[0] as GridControl).ItemsSource = aSelectedTradeFormula.modularFormulaBodiesHavingQuoteinpriceQuote;
                }
                List<WeightedComponent> weightComponents = new List<WeightedComponent>();
                List<QuoteComponent> QComponents = new List<QuoteComponent>();
                List<EstimatesComponent> EComponents = new List<EstimatesComponent>();
                formulaComps = formulaComps.OrderBy(o => o.formulaCompName).ToList<FormulaComponent>();
                foreach (FormulaComponent fComp in formulaComps)
                {
                    if (fComp.formulaCompType != null && fComp.formulaCompType.Equals("S"))
                        continue;
                    if (fComp.isTypeWeight())
                    {
                        WeightedComponent weighComp = new WeightedComponent();
                        weighComp.DataContext = fComp;
                        weighComp.selectedTradeFormula = aSelectedTradeFormula;
                        //lineItemsCtl.stkComponents.Children.Add(weighComp);
                        weightComponents.Add(weighComp);
                        //formulaStackpanel.Children.Add(weighComp);
                    }
                    else if (fComp.formulaCompType.Equals("G"))
                    {
                        QuoteComponent quoteComp = new QuoteComponent();
                        quoteComp.DataContext = fComp;
                        quoteComp.selectedTradeFormula = aSelectedTradeFormula;
                        //formulaStackpanel.Children.Add(quoteComp);
                        QComponents.Add(quoteComp);
                    }
                    else
                    {
                        EstimatesComponent estimateComp = new EstimatesComponent();
                        estimateComp.DataContext = fComp;
                        estimateComp.selectedTradeFormula = aSelectedTradeFormula;
                        EComponents.Add(estimateComp);
                        //formulaStackpanel.Children.Add(estimateComp);
                    }
                }
                foreach (QuoteComponent Qcomp in QComponents)
                {
                    formulaStackpanel.Children.Add(Qcomp);
                }

                foreach (EstimatesComponent Ecomp in EComponents)
                {
                    formulaStackpanel.Children.Add(Ecomp);
                }

                foreach (WeightedComponent wComp in weightComponents)
                {
                    formulaStackpanel.Children.Add(wComp);
                }
            }
        }

        private static void addFCCtrlsToModulatLineItemCtrl(List<FormulaComponent> formulaComps, ModularLineItemComponentsCtl lineItemsCtl, Formula aTradeFormula)
        {
            int priceLineItemCount = 0;
            if (aTradeFormula != null && formulaComps != null && formulaComps.Count() > 0
                && lineItemsCtl != null)
            {
                if (aTradeFormula.formulaBodies != null)
                    priceLineItemCount = aTradeFormula.formulaBodies.Where(fb => fb.formulaBodyType.Equals("P")).Count();

                List<WeightedComponent> weightComponents = new List<WeightedComponent>();
                List<QuoteComponent> QComponents = new List<QuoteComponent>();
                List<EstimatesComponent> EComponents = new List<EstimatesComponent>();
                formulaComps = formulaComps.OrderBy(o => o.formulaCompName).ToList<FormulaComponent>();
                foreach (FormulaComponent fComp in formulaComps)
                {
                    fComp.makeNewFC = false;
                    if (fComp.formulaCompType != null && fComp.formulaCompType.Equals("S"))
                        continue;//Spec related comp values are not editable , hence we are not shows here . Specs are editable at specification in TradeWindow
                    if (fComp.isTypeWeight())
                    {
                        WeightedComponent weighComp = new WeightedComponent();
                        weighComp.DataContext = fComp;
                        weighComp.selectedTradeFormula = aTradeFormula;

                        //lineItemsCtl.stkComponents.Children.Add(weighComp);
                        //By default visibility is false
                        //if (priceLineItemCount == 0)
                        //{
                        //    weighComp.chkBoxMakeNewFC.Visibility = Visibility.Hidden;                            
                        //}
                        //else
                        //{
                        //fComp.makeNewFC = false;
                        if (aTradeFormula.isComponentExists(fComp))
                        {
                            //fComp.makeNewFC = false;//ie use existing FC on trade
                            weighComp.chkBoxMakeNewFCVisible();
                            weighComp.enableOrDisableControls(false);
                        }
                        weightComponents.Add(weighComp);
                        //}

                    }
                    else if (fComp.formulaCompType.Equals("G"))
                    {
                        QuoteComponent quoteComp = new QuoteComponent();
                        quoteComp.DataContext = fComp;
                        quoteComp.selectedTradeFormula = aTradeFormula;
                        //lineItemsCtl.stkComponents.Children.Add(quoteComp);
                        //By default visibility is false
                        //if (priceLineItemCount == 0)
                        //{
                        //    quoteComp.chkBoxMakeNewFC.Visibility = Visibility.Hidden;
                        //}
                        //else
                        //{
                        if (aTradeFormula.isComponentExists(fComp))
                        {
                            //fComp.makeNewFC = false;//ie use existing FC on trade
                            quoteComp.chkBoxMakeNewFCVisible();
                            quoteComp.enableOrDisableControls(false);

                        }
                        //}
                        QComponents.Add(quoteComp);

                    }
                    else
                    {
                        EstimatesComponent estimateComp = new EstimatesComponent();
                        estimateComp.DataContext = fComp;
                        estimateComp.selectedTradeFormula = aTradeFormula;
                        //lineItemsCtl.stkComponents.Children.Add(estimateComp);
                        //By default visibility is false
                        //if (priceLineItemCount == 0)
                        //{
                        //    estimateComp.chkBoxMakeNewFC.Visibility = Visibility.Hidden;                           
                        //}
                        //else
                        //{
                        if (aTradeFormula.isComponentExists(fComp))
                        {
                            //fComp.makeNewFC = false;//ie use existing FC on trade
                            estimateComp.chkBoxMakeNewFCVisible();
                            estimateComp.enableOrDisableControls(false);
                        }
                        //}
                        EComponents.Add(estimateComp);

                    }
                }

                foreach (QuoteComponent Qcomp in QComponents)
                {
                    lineItemsCtl.stkComponents.Children.Add(Qcomp);
                }

                foreach (EstimatesComponent Ecomp in EComponents)
                {
                    lineItemsCtl.stkComponents.Children.Add(Ecomp);
                }

                foreach (WeightedComponent wComp in weightComponents)
                {
                    lineItemsCtl.stkComponents.Children.Add(wComp);
                }
            }
        }

        private static void editFCCtrlsToModulatLineItemCtrl(List<FormulaComponent> formulaComps, ModularLineItemComponentsCtl lineItemsCtl)
        {
            if (formulaComps != null && formulaComps.Count() > 0
                && lineItemsCtl != null)
            {
                formulaComps = formulaComps.OrderBy(o => o.formulaCompName).ToList<FormulaComponent>();
                foreach (FormulaComponent fComp in formulaComps)
                {
                    if (fComp.formulaCompType != null && fComp.formulaCompType.Equals("S"))
                        continue;//Spec related comp values are not editable , hence we are not shows here . Specs are editable at specification in TradeWindow
                    if (fComp.isTypeWeight())
                    {
                        WeightedComponent weighComp = new WeightedComponent();
                        weighComp.DataContext = fComp;

                        lineItemsCtl.stkComponents.Children.Add(weighComp);
                    }
                    else if (fComp.formulaCompType.Equals("G"))
                    {
                        QuoteComponent quoteComp = new QuoteComponent();
                        quoteComp.DataContext = fComp;
                        lineItemsCtl.stkComponents.Children.Add(quoteComp);
                    }
                    else
                    {
                        EstimatesComponent estimateComp = new EstimatesComponent();
                        estimateComp.DataContext = fComp;
                        lineItemsCtl.stkComponents.Children.Add(estimateComp);
                    }
                }
            }
        }
        public void OpenFormulaPanelByFormulaNum(object formula)
        {
            string formulaNums = null;

            if (formula != null && formula is Formula)
            {
                formulaNums = (formula as Formula).formulaNum.ToString();
            }
            IDataStore dataStoreService = DataStoreServiceHelper.createNewDataStoreService();
            cs.fw.infra.service.IDataStoreService dataStore = (cs.fw.infra.service.IDataStoreService)dataStoreService;
            //IDataStoreService dataStore = container.Resolve<IDataStoreService>();
            ICacheService cacheService = new CacheService(container, (IDataStoreService)dataStore);
            if (formulaNums != null && formulaNums.Trim().Length > 0)
            {
                String[] oidStrings = formulaNums.Trim().Split(',');
                int oid = 0;
                if (oidStrings != null && oidStrings.Length > 0)
                {
                    foreach (String oidString in oidStrings)
                    {
                        if (oidString != null && !oidString.Trim().Equals(string.Empty))
                        {
                            try
                            {
                                oid = Convert.ToInt32(oidString);
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Invalid Trade Number: " + oidString, null);
                            }
                            if (oid > 0)
                            {
                                try
                                {
                                    string mergeType = GenericRecord.UserChangesMergeOverwriteUserChanges;
                                    IMybatisDataAccessService MybatisDataAccessService = container.Resolve<IMybatisDataAccessService>();
                                    CSFetchSpecification fetchSpecification = new CSFetchSpecification("Formula", "Tradelayout", SpecificationTypes.System);
                                    CSKeyValueQualifier qualifier = new CSKeyValueQualifier("f.formula_num", CSQualifierOperatorSelectors.QualifierOperatorEqual, oid);
                                    fetchSpecification.Qualifier = qualifier;
                                    List<Formula> formulas = MybatisDataAccessService.FetchForQuery<Formula>(fetchSpecification, true, true, cs.fw.eo.EOUtil.GetResultMapName("Formula"), true, null, true, dataStore);
                                    string isFromTradeFormPanel = string.Empty;
                                    if (formulas == null || formulas.Count == 0)
                                    {
                                        MessageBox.Show("Formula # " + oid + " not found!", null, MessageBoxButton.OK, MessageBoxImage.Information);
                                        return;
                                    }
                                    else
                                    {
                                        if (formulas.Count > 0 && formulas[0].formulaUse != null)
                                        {
                                            formulas[0].dataStoreService = dataStore;
                                            //foreach (FormulaBody fb in formulas[0].formulaBodies)
                                            //    fb.SetPropertyWithOutAddingToChanges<Formula>(formulas[0], "formula");
                                            if (formulas[0].modularInd != null && formulas[0].modularInd.Equals("Y"))
                                            {
                                                if (formulas[0].formulaUse.Equals("D"))
                                                    showModularLineItemFormulaEditor(formulas[0]);
                                                else
                                                    ShowModularFormulaEditor(formulas[0]);
                                            }
                                            else
                                                ShowComplexFormulaEditor(formulas[0]);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Trade # " + oidString + " couldn't be opened because of the following reason.\n" + ex.Message, null, MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void showModularLineItemFormulaEditor(Formula modularFormula)
        {
            ModularLineItemEditor modularLineItemEditor = new ModularLineItemEditor();
            modularLineItemEditor.DataContext = modularFormula;
            modularLineItemEditor.container = this.container;
            modularLineItemEditor.priceLineItemEditor.DataContext = modularFormula.formulaBodies[0];
            modularLineItemEditor.priceLineItemEditor.btnClose.Visibility = Visibility.Collapsed;
            string title = string.Empty;
            if (modularFormula.isInDb)
            {
                modularLineItemEditor.btnSaveAs.IsEnabled = true;
                modularLineItemEditor.btnDelete.IsEnabled = true;
            }
            else
            {
                modularLineItemEditor.btnSaveAs.IsEnabled = false;
                modularLineItemEditor.btnDelete.IsEnabled = false;
            }
            if (newFormulaOptions != null)
            {
                if (newFormulaOptions.FormulaPrecision != null)
                    modularFormula.formulaPrecision = newFormulaOptions.FormulaPrecision;
                if (newFormulaOptions.FormulaUomCode != null && !newFormulaOptions.FormulaUomCode.Equals(string.Empty))
                {
                    modularFormula.formulaUomCode = newFormulaOptions.FormulaUomCode;
                    modularFormula.uom = newFormulaOptions.FormulaUom;
                    modularLineItemEditor.dFormulaUom.lookupEdit.SelectedItem = newFormulaOptions.FormulaUom;
                }
                if (newFormulaOptions.FormulaCurrCode != null && !newFormulaOptions.FormulaCurrCode.Equals(string.Empty))
                {
                    modularFormula.formulaCurrCode = newFormulaOptions.FormulaCurrCode;
                    modularFormula.currency = newFormulaOptions.FormulaCurrency;
                    modularLineItemEditor.dFormulaCurrency.lookupEdit.SelectedItem = newFormulaOptions.FormulaCurrency;
                }
            }
            //if (formula != null && formula.formulaName != null && !formula.formulaName.Equals(string.Empty))
            //    title = formula.formulaName + ": formula is priced in " + formula.formulaCurrCode + "/" + formula.formulaUomCode + " with Precision: " + formula.formulaPrecision;
            //else
            title = "This formula is priced in " + (modularFormula.currency != null ? modularFormula.currency.cmdtyShortName : null) + "/" + modularFormula.formulaUomCode + " with Precision: " + modularFormula.formulaPrecision;
            IPopupService popupService = container.Resolve<IPopupService>();
            popupService.AddPopup(title, modularLineItemEditor, false, false);
        }


        CreateNewFormula newFormulaPopup = null;
        NewFormulaOptions newFormulaOptions = null;
        public void OpenCreateLibFormula(object assocTradeCtl)
        {
            newFormulaOptions = null;
            newFormulaPopup = container.Resolve<CreateNewFormula>();
            newFormulaPopup.container = container;
            newFormulaPopup.gpbCreationtype.Visibility = Visibility.Collapsed;
            newFormulaPopup.canHaveModularFormula = true;
            newFormulaPopup.canHaveComplexFormula = true;
            newFormulaPopup.NewSelectedFormulaOptions += new EventHandler<NewFormulaOptionsSelected>(FormulaOptionsSelected);

            //setWindowBlurrEffect();
            IPopupService popupService = container.Resolve<IPopupService>();

            //below line modified for issue 1400319
            //popupService.AddPopup("Create New Library Formula", newFormulaPopup);
            popupService.AddPopup("Create New Library Formula", newFormulaPopup, false, false);

            //removeWindowBlurrEffect();
        }

        public void FormulaOptionsSelected(object sender, NewFormulaOptionsSelected e)
        {
            if (newFormulaPopup != null)
            {
                newFormulaPopup.NewSelectedFormulaOptions -= new EventHandler<NewFormulaOptionsSelected>(FormulaOptionsSelected);
            }
            newFormulaOptions = e.NewFormulaSelectedOptions;
            bool isComplexFormula = false;
            string formulaTypeInd = string.Empty;
            if (newFormulaOptions.SimpleFormula == true)
                formulaTypeInd = "S";
            else if (newFormulaOptions.ComplexFormula == true)
            {
                formulaTypeInd = "C";
                isComplexFormula = true;
            }
            else
            {
                formulaTypeInd = "M";
                isComplexFormula = true;
            }
            if (newFormulaOptions.ModularLineItemFormula == true)
            {
                Formula modularFormula = FormulaBusiness.createModularLineItemFormula();
                showModularLineItemFormulaEditor(modularFormula);
            }
            else
            {
                Formula libFormula = FormulaBusiness.CreateLibraryFormula(newFormulaOptions.CreationType, formulaTypeInd, isComplexFormula);
                if (libFormula.formulaUse.Equals("L"))
                {
                    if (formulaTypeInd.Equals("C"))
                        ShowComplexFormulaEditor(libFormula);
                    else if (formulaTypeInd.Equals("M"))
                        ShowModularFormulaEditor(libFormula);
                }
            }
        }

        private void ShowSimpleFormulaEditor(Formula formula)
        {
            formula.setUpFormula();
            SimpleFormulaEditor sf = container.Resolve<SimpleFormulaEditor>();
            sf.DataContext = formula;
            sf.container = this.container;
            string title = string.Empty;
            if (newFormulaOptions != null)
            {
                if (newFormulaOptions.FormulaPrecision != null)
                    formula.formulaPrecision = newFormulaOptions.FormulaPrecision;
                if (newFormulaOptions.FormulaUomCode != null && !newFormulaOptions.FormulaUomCode.Equals(string.Empty))
                {
                    formula.formulaUomCode = newFormulaOptions.FormulaUomCode;
                    sf.dpcUomCode.lookupEdit.SelectedItem = newFormulaOptions.FormulaUom;
                }
                if (newFormulaOptions.FormulaCurrCode != null && !newFormulaOptions.FormulaCurrCode.Equals(string.Empty))
                {
                    formula.formulaCurrCode = newFormulaOptions.FormulaCurrCode;
                    sf.dpcCurrencyCode.lookupEdit.SelectedItem = newFormulaOptions.FormulaCurrency;
                }
            }
            if (formula != null && formula.formulaName != null && !formula.formulaName.Equals(string.Empty))
                title = formula.formulaName + ": formula will be priced in " + (formula.currency != null ? formula.currency.cmdtyShortName : null) + "/" + formula.formulaUomCode + " with Precision: " + formula.formulaPrecision;
            else
                title = "This formula will be priced in " + (formula.currency != null ? formula.currency.cmdtyShortName : null) + "/" + formula.formulaUomCode + " with Precision: " + formula.formulaPrecision;
            //setWindowBlurrEffect();
            IPopupService popupService = container.Resolve<IPopupService>();

            //below line modified for issue 1400319
            //popupService.AddPopup(title, sf);
            popupService.AddPopup(title, sf, false, false);

            //removeWindowBlurrEffect();
        }
        private void ShowComplexFormulaEditor(Formula formula)
        {
            formula.setUpFormula();
            if (formula.premiumBody == null)
                formula.AddToFormulaBodies(formula.CreatePremiumBody());
            if (formula.premiumBody != null)
                formula.premiumBody.Parser = formula.sharedParser;
            ComplexFormulaEditor cf = container.Resolve<ComplexFormulaEditor>();
            cf.gpbFormulaPricingUnits.Visibility = Visibility.Visible;
            cf.lblThis.Visibility = Visibility.Visible;
            cf.cmbLibraryFormulaUse.Visibility = Visibility.Visible;
            cf.btnOkCmplxFormula.Visibility = Visibility.Collapsed;
            cf.btnSaveCmplxFormula.Visibility = Visibility.Visible;
            cf.btnDeleteCmplxFormula.Visibility = Visibility.Visible;
            cf.btnSaveASModFormula.Visibility = Visibility.Visible;
            if (formula.isInDb)
            {
                cf.btnSaveASModFormula.IsEnabled = true;
            }
            else
            {
                cf.btnSaveASModFormula.IsEnabled = false;
            }
            cf.DataContext = formula;
            cf.container = this.container;
            string title = string.Empty;
            if (newFormulaOptions != null)
            {
                if (newFormulaOptions.FormulaPrecision != null)
                    formula.formulaPrecision = newFormulaOptions.FormulaPrecision;
                if (newFormulaOptions.FormulaUomCode != null && !newFormulaOptions.FormulaUomCode.Equals(string.Empty))
                {
                    formula.formulaUomCode = newFormulaOptions.FormulaUomCode;
                    cf.dpcUomCode.lookupEdit.SelectedItem = newFormulaOptions.FormulaUom;
                }
                if (newFormulaOptions.FormulaCurrCode != null && !newFormulaOptions.FormulaCurrCode.Equals(string.Empty))
                {
                    formula.formulaCurrCode = newFormulaOptions.FormulaCurrCode;
                    cf.dpcCurrencyCode.lookupEdit.SelectedItem = newFormulaOptions.FormulaCurrency;
                }
            }
            if (formula != null && formula.formulaName != null && !formula.formulaName.Equals(string.Empty))
                title = formula.formulaName + ": formula is priced in " + (formula.currency != null ? formula.currency.cmdtyShortName : null) + "/" + formula.formulaUomCode + " with Precision: " + formula.formulaPrecision;
            else
                title = "This formula is priced in " + (formula.currency != null ? formula.currency.cmdtyShortName : null) + "/" + formula.formulaUomCode + " with Precision: " + formula.formulaPrecision;
            //setWindowBlurrEffect();
            IPopupService popupService = container.Resolve<IPopupService>();

            //below line modified for issue 1400319
            //popupService.AddPopup(title, cf);
            popupService.AddPopup(title, cf, false, false);
            //removeWindowBlurrEffect();
        }
        private void ShowModularFormulaEditor(Formula formula)
        {
            formula.setUpFormula();
            if (formula.premiumBody != null)
                formula.premiumBody.Parser = formula.sharedParser;
            ModularFormulaEditor cf = container.Resolve<ModularFormulaEditor>();
            cf.gpbFormulaPricingUnits.Visibility = Visibility.Visible;
            cf.lblThis.Visibility = Visibility.Visible;
            cf.cmbLibraryFormulaUse.Visibility = Visibility.Visible;
            cf.btnOkCmplxFormula.Visibility = Visibility.Collapsed;
            cf.btnSaveModFormula.Visibility = Visibility.Visible;
            cf.btnDeleteCmplxFormula.Visibility = Visibility.Visible;
            cf.btnSaveASModFormula.Visibility = Visibility.Visible;
            if (formula.isInDb)
            {
                cf.btnSaveASModFormula.IsEnabled = true;
                cf.btnDeleteCmplxFormula.IsEnabled = true;
            }
            else
            {
                cf.btnSaveASModFormula.IsEnabled = false;
                cf.btnDeleteCmplxFormula.IsEnabled = false;
            }
            cf.DataContext = formula;
            cf.container = this.container;
            string title = string.Empty;
            if (newFormulaOptions != null)
            {
                if (newFormulaOptions.FormulaPrecision != null)
                    formula.formulaPrecision = newFormulaOptions.FormulaPrecision;
                if (newFormulaOptions.FormulaUomCode != null && !newFormulaOptions.FormulaUomCode.Equals(string.Empty))
                {
                    formula.formulaUomCode = newFormulaOptions.FormulaUomCode;
                    cf.dpcUomCode.lookupEdit.SelectedItem = newFormulaOptions.FormulaUom;
                }
                if (newFormulaOptions.FormulaCurrCode != null && !newFormulaOptions.FormulaCurrCode.Equals(string.Empty))
                {
                    formula.formulaCurrCode = newFormulaOptions.FormulaCurrCode;
                    cf.dpcCurrencyCode.lookupEdit.SelectedItem = newFormulaOptions.FormulaCurrency;
                }
            }
            if (formula != null && formula.formulaName != null && !formula.formulaName.Equals(string.Empty))
                title = formula.formulaName + ": formula is priced in " + (formula.currency != null ? formula.currency.cmdtyShortName : null) + "/" + formula.formulaUomCode + " with Precision: " + formula.formulaPrecision;
            else
                title = "This formula is priced in " + (formula.currency != null ? formula.currency.cmdtyShortName : null) + "/" + formula.formulaUomCode + " with Precision: " + formula.formulaPrecision;
            //setWindowBlurrEffect();
            IPopupService popupService = container.Resolve<IPopupService>();

            //below line modified for issue 1400319
            //popupService.AddPopup(title, cf);
            popupService.AddPopup(title, cf, false, false);

            //removeWindowBlurrEffect();
        }

        public void ApplyChangesToDetail(UserControl userCtl)
        {
            if (userCtl.DataContext != null && userCtl.DataContext is Trade)
            {
                TradePanelCtl tableCtl = VisualTreeUtil.FindVisualParent<TradePanelCtl>(userCtl);
                if (tableCtl != null)
                    tableCtl.IsBeingModifiedBySystem = true;
                try
                {
                    Trade trade = (userCtl.DataContext as Trade);
                    IDataStoreService dataService = (IDataStoreService)trade.dataStoreService;
                    TradeOrder ato;
                    if (trade.tradeOrders != null && trade.tradeOrders.Count > 0)
                    {
                        for (int i = 0; i < trade.tradeOrders.Count; i++)
                        {
                            ato = trade.tradeOrders[i];
                            
                            if (ato.tradeItems != null && ato.tradeItems.Count > 0)
                            {
                                for (int j = 0; j < ato.tradeItems.Count; j++)
                                {
                                    //added a check to avoid unnecessary db hit if it is a an unsaved trade while working on issue 1397016.
                                    TradeItem anTI = ato.tradeItems[j];
                                    if (anTI != null && anTI.isInDb)
                                    {
                                        anTI.getDetailDataViaIBatis(container);
                                    }
										if (ato.IsSummary)//|| (genericRecord as Trade).SelectedTradeItem.IsDirty))///|| [_tradeItem.tradeiteme isDirty])))
										{
											ato.updateStripDetailCount(this.container);
											if (ato.stripDetailOrderCount < 1)
											{
												MessageBox.Show("The month of Strip-To cannot be earlier that the first " +
                                                     ((ato.tradeItems[0].itemType.Equals(TradeConstants.WETPHYSICAL) || ato.tradeItems[0].itemType.Equals(TradeConstants.DRYPHYSICAL)) ? "Delivery" : "Pricing") +
                                                     " Month for strip order " + ato.orderNum,
                                                     "Trade Capture", MessageBoxButton.OK
                                                    );
												return;
											}
										}
                                        //code added to handle delete existing formula and added new formula & //[I#ADSO-3680/MERCC-42] condition modified to skip deleting formula on detail order when anyCostVouched/AiScheduled/EFPOrderFilled.
                                        if (ato.IsStrip && !string.IsNullOrEmpty(anTI.deleteDetailOrdersFormulaInd) && !anTI.isAnyCostVouched && !anTI.isAIScheduled() && !anTI.isEFPOrderFilled())
                                        {
                                            if (!anTI.deleteDetailOrdersFormulaInd.Contains("_"))
                                                anTI.deleteDetailOrdersFormulaInd = anTI.deleteDetailOrdersFormulaInd + "_";
                                            foreach (string formulaToDelete in anTI.deleteDetailOrdersFormulaInd.Split('_'))
                                            {
                                                if (!string.IsNullOrEmpty(formulaToDelete))
                                                {
                                                    bool containsModifiedFormula = false;
                                                    if (anTI.tradeItemModFields != null && anTI.tradeItemModFields.Count > 0)
                                                    {
                                                        foreach (TiFieldModified aField in anTI.tradeItemModFields)
                                                        {
                                                            if (aField.attrName.Equals("mainFormula"))// fixed for mainformula only, need to fix for prelim,market formulas
                                                            {
                                                                containsModifiedFormula = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    if (!containsModifiedFormula)
                                                    {
                                                        if (formulaToDelete.Equals("mainformula"))
                                                        {
                                                            if (anTI.mainformula != null && anTI.mainformula.isInDb)
                                                            {
                                                                dataService.AddToDeletedObjectsList(anTI.mainformula);
                                                                dataService.AddToDeletedObjectsList(anTI.mainformula.tradeFormula[0]);
                                                                anTI.RemoveFromTradeFormulas(anTI.mainformula.tradeFormula[0]);
                                                            }
                                                        }
                                                        if (formulaToDelete.Equals("marketformula"))
                                                        {
                                                            if (anTI.marketformula != null && anTI.marketformula.isInDb)
                                                            {
                                                                dataService.AddToDeletedObjectsList(anTI.marketformula);
                                                                dataService.AddToDeletedObjectsList(anTI.marketformula.tradeFormula[0]);
                                                                anTI.RemoveFromTradeFormulas(anTI.marketformula.tradeFormula[0]);
                                                            }
                                                        }
                                                        if (formulaToDelete.Equals("prelimformula"))
                                                        {
                                                            if (anTI.prelimformula != null && anTI.prelimformula.isInDb)
                                                            {
                                                                dataService.AddToDeletedObjectsList(anTI.prelimformula);
                                                                dataService.AddToDeletedObjectsList(anTI.prelimformula.tradeFormula[0]);
                                                                anTI.RemoveFromTradeFormulas(anTI.prelimformula.tradeFormula[0]);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            anTI.deleteDetailOrdersFormulaInd = null;
                                        }
                                   // }
                                }
                            }
                        }
                    }
                    if (reStrip(userCtl))
                    {
                        MessageBoxResult msgResult = MessageBoxResult.No;
                        msgResult = MessageBox.Show("Cannot update all the detail orders of this strip template order, because some of the detail orders have been scheduled or their costs have been vouched.  Do you want to update the detail orders which have not been scheduled and their costs have not been vouched?", "Trade Capture", MessageBoxButton.YesNo);
                        if (msgResult == MessageBoxResult.No)
                            return;
                    }
                    /*ViewState viewState = dataService.CreateViewState();
                    List<GenericMessage> listOfMessages = new List<GenericMessage>();
                    ApplyChangesToDetails applyChanges = new ApplyChangesToDetails();
                    if (trade.isInDb == false)
                        applyChanges.TradeRecordKey = trade.RecordKey;
                    else
                    {
                        applyChanges.TradeNum = trade.tradeNum.Value;
                        if (trade.SelectedTradeItem != null)
                            applyChanges.OrderNum = trade.SelectedTradeItem.tradeOrder.orderNum.Value;
                        trade.tradeModDate = DateTime.Now;
                        ISecurityService securityService = container.Resolve<ISecurityService>();
                        com.tc.frameworks.ictseos.eoaccount.IctsUser user = securityService.LoggedInUser as com.tc.frameworks.ictseos.eoaccount.IctsUser;
                        trade.tradeModInit = securityService.LoggedInUserInit;
                    }
                    listOfMessages.Add(applyChanges);
                    ProcessApplyChangesTodetails(listOfMessages, viewState, dataService, trade, userCtl);*/

                    ProcessApplyChanges(dataService, trade, userCtl);
                }
                finally
                {
                    if (tableCtl != null)
                        tableCtl.IsBeingModifiedBySystem = false;
                }
            }
        }

        //[I#ADSO-1049] code added to check each trade item for anycostvouched/scheduled [Based on OC reference]
        private bool reStrip(UserControl userCtl)
        {
            bool flag = false;
            Trade trade = (userCtl.DataContext as Trade);
            if (trade.TradeItems != null && trade.TradeItems.Count > 0)
            {
                foreach (TradeItem anTi in trade.TradeItems)
                {
                    if (anTi.isAnyCostVouched || anTi.isAIScheduled())
                    {
                        flag = true;
                        break;
                    }
                }
            }
            return flag;
        }


        //we come here first time the apply changes is called.          
        private void ProcessApplyChanges(IDataStoreService dataService, GenericRecord trade, UserControl userCtl)
        {
            ProcessApplyChanges(dataService, trade, userCtl, 1);
        }

        //to handle stale objects attemptno is added
        private void ProcessApplyChanges(IDataStoreService dataService, GenericRecord trade, UserControl userCtl, int attemptno)
        {
            ViewState viewState = dataService.CreateViewState();
            List<GenericMessage> listOfMessages = new List<GenericMessage>();
            ApplyChangesToDetails applyChanges = new ApplyChangesToDetails();
            if (trade.isInDb == false)
                applyChanges.TradeRecordKey = trade.RecordKey;
            else
            {
                applyChanges.TradeNum = (trade as Trade).tradeNum.Value;
                if ((trade as Trade).SelectedTradeItem != null)
                    applyChanges.OrderNum = (trade as Trade).SelectedTradeItem.tradeOrder.orderNum.Value;
                (trade as Trade).tradeModDate = DateTime.Now;
                ISecurityService securityService = container.Resolve<ISecurityService>();
                com.tc.frameworks.ictseos.eoaccount.IctsUser user = securityService.LoggedInUser as com.tc.frameworks.ictseos.eoaccount.IctsUser;
                (trade as Trade).tradeModInit = securityService.LoggedInUserInit;
            }
            listOfMessages.Add(applyChanges);
            ProcessApplyChangesTodetails(listOfMessages, viewState, dataService, trade, userCtl, attemptno);
        }

        private void ProcessApplyChangesTodetails(List<GenericMessage> listOfMessages, ViewState viewState,
            IDataStoreService dataService, GenericRecord genericRecord, UserControl userCtl, int attemptno)
        {
            Trade _trade = genericRecord as Trade;
            _trade.sentForApplyChanges = true;
            string jBossMachineName = null;
            string failurMsg = string.Empty;
            StringBuilder sb = new StringBuilder();
            string successMsg = string.Empty;
            CompositeMessage message = new CompositeMessage();
            message.Messages = listOfMessages;
            IMessageProcessorService processService = container.Resolve<IMessageProcessorService>();
            CompositeResponseMessage response = null;

            ValidationMessageHolder.Instance.RemoveFailureAndWarningValidationMessage();
            if (System.Windows.Forms.Cursor.Current == System.Windows.Forms.Cursors.Default)
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            response = processService.ProcessApplyChanges(message, viewState, dataService);
            if (System.Windows.Forms.Cursor.Current == System.Windows.Forms.Cursors.WaitCursor)
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;

            foreach (ResponseMessage resposeMsg in response.responseMessages)
            {
                GenericMessage sentMessage = message[resposeMsg.correlationId];
                if (sentMessage != null)
                {
                    /*if (resposeMsg.statusMessage.Contains("Failure"))
                    {
                        jBossMachineName = resposeMsg.machineName;
                        failurMsg = "Apply Changes Failed : " + System.Environment.NewLine;
                        if (resposeMsg.validationResults != null)
                        {
                            failurMsg = string.Empty;
                            foreach (tc.bean.validation.ValidationResult vr in response.responseMessages[0].validationResults.results)
                            {
                                //failurMsg += vr.ToString() + System.Environment.NewLine;
                                ValidationMessageHolder.Instance.AddValidationMessage(vr.validationName, vr.validationMessage, Severity.Failure);
                            }
                            //MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.OK);
                        }
                    }*/

                    if (resposeMsg.statusMessage.Contains("Failure"))
                    {
                        jBossMachineName = resposeMsg.machineName;
                        failurMsg = "Apply Changes Failed : " + System.Environment.NewLine;
                        sb.Append(resposeMsg.statusMessage).Append(Environment.NewLine);

                        if (resposeMsg.HasStaleObjects())
                        {
                            foreach (string recordKey in resposeMsg.staleObjectsRecordKey)
                            {
                                EntityRecord localGenericRecord = dataService.GetGenericObject(recordKey) as EntityRecord;
                                if (localGenericRecord != null)
                                {
                                    //ServiceUtil.FetchStaleObject<EntityRecord>(localGenericRecord);
                                    ServiceUtil.FetchStaleObject<EntityRecord>(localGenericRecord, dataService);
                                    int? transId = null;

                                    try
                                    {
                                        transId = Convert.ToInt32(localGenericRecord.GetType().GetProperty("transId").GetValue(localGenericRecord, null));
                                    }
                                    catch (Exception e)
                                    {
                                    }

                                    string logmsg = "StaleObject -> " + recordKey + " New TransId : " + transId;
                                }
                            }

                            if (attemptno == 1)
                            {
                                failurMsg = "Data was modified by another process/user, apply changes with refreshed data is being attempted";
                                ValidationMessageHolder.Instance.AddValidationMessage(failurMsg, failurMsg, Severity.Failure);
                            }

                            if (attemptno <= 5)
                            {
                                attemptno++;
                                ProcessApplyChangesTodetails(listOfMessages, viewState, dataService, genericRecord, userCtl, attemptno);
                            }
                            else
                            {
                                failurMsg = "Data was modified by another process/user, apply changes with refreshed data was attempted, Please reopen";
                                //ValidationMessageHolder.Instance.AddValidationMessage(failurMsg, failurMsg, Severity.Failure);
                                MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.OK);
                            }
                            return;
                        }
                        else
                        {
                            if (resposeMsg.validationResults != null)
                            {
                                failurMsg = string.Empty;
                                bool staleObjectValidation = false;
                                //looping through all the validation results and adding to the display message
                                foreach (tc.bean.validation.ValidationResult vr in response.responseMessages[0].validationResults.results)
                                {
                                    if (vr.validationMessage.Contains("Row was updated or deleted by another transaction"))
                                    {
                                        staleObjectValidation = true;
                                        break;
                                    }
                                    ValidationMessageHolder.Instance.AddValidationMessage(vr.validationName, vr.validationMessage, Severity.Failure);
                                    //failurMsg += vr.ToString() + System.Environment.NewLine;
                                }

                                if (staleObjectValidation)
                                {
                                    if (attemptno == 1)
                                    {
                                        failurMsg = "Data was modified by another process/user, apply changes with refreshed data is being attempted";
                                        ValidationMessageHolder.Instance.AddValidationMessage(failurMsg, failurMsg, Severity.Failure);
                                    }

                                    if (attemptno <= 5)
                                    {
                                        attemptno++;
                                        ProcessApplyChangesTodetails(listOfMessages, viewState, dataService, genericRecord, userCtl, attemptno);
                                    }
                                    else
                                    {
                                        failurMsg = "Data was modified by another process/user, apply changes with refreshed data was attempted, Please reopen";
                                        //ValidationMessageHolder.Instance.AddValidationMessage(failurMsg, failurMsg, Severity.Failure);
                                        MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.OK);
                                    }
                                }
                                //MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.OK);
                            }
                        }
                    }//end of failure block
                    else if (resposeMsg.statusMessage.Contains("Success"))
                    {
                        MessageBox.Show("Applied changes to the details orders successfully");
                        Trade tr = (genericRecord as Trade);
                        if (tr.tradeOrders != null && tr.TradeItems.Count > 0)
                        {
                            foreach (TradeOrder to in tr.tradeOrders)
                            {
                                foreach (TradeItem ti in to.tradeItems)
                                {
                                    //changes by Raju K on 10Jul14
                                    //I#1397338 - Tradingprd is null due to RiskTradeingPrd is null. Here filling RiskTradingPeriod again to avoid right trading prd.
                                    //[I#ADSO-555] -- Commented the below condition and 
                                    //implemented the logic inside setRiskTradingPeriod() , which is similar as OC
                                    //if (ti.riskTradingPeriod == null && ti.tradingPrd != null)
                                    ti.setRiskTradingPeriod();
                                    if (ti.costs != null && ti.costs.Count > 0)
                                    {
                                        foreach (Cost acost in ti.costs)
                                        {
                                            if (acost.costAmtType != null && acost.costAmtType.Equals("f") && acost.accumulation != null)
                                            {
                                                acost.ResetAccumulation();
                                            }
                                        }
                                    }
                                    List<Formula> _subformulalist = new List<Formula>();
                                    if (ti is CashPhysicalTradeItem && ti.mainformula != null && ti.mainformula.formulaBodies != null && ti.mainformula.formulaBodies.Count > 0)
                                    {
                                        foreach (FormulaBody Fb in ti.mainformula.formulaBodies)
                                        {
                                            if (Fb.formulaBodyType.Equals("P") && Fb.formulaComponents != null && Fb.formulaComponents.Count > 0)
                                            {
                                                foreach (FormulaComponent fc in Fb.formulaComponents)
                                                {
                                                    if (fc.subFormula != null)
                                                        _subformulalist.Add(fc.subFormula);
                                                }
                                            }
                                        }
                                        if (_subformulalist != null && _subformulalist.Count > 0)
                                            ti.mainformula.subFormulaArray = _subformulalist;
                                    }
                                }
                                if (to.IsSummary && to.IsApplyChanges)
                                {
                                    to.IsApplyChanges = false;
                                    to.NotifyPropertyChanged("IsApplyChanges");
                                }
                            }
                        }
                        (userCtl as StripModePanelCtl).rbtnDetails.IsChecked = true;
                        _trade.sentForApplyChanges = false;
                    }
                }
            }
        }
        //public void OpenNewTradePanel(GenericRecord selectedRecord)
        //{
        //    PanelCommandParamter panelCommandParameter = new PanelCommandParamter();
        //    panelCommandParameter.Record = selectedRecord;
        //    FutureTradeCommands.OpenNewTradePanelCommand.Execute(panelCommandParameter);
        //}

        /// <summary>
        /// This method will open the List of  Trade form panel by comma seperated Trade nums
        /// </summary>
        /// <param name="tradeNums"></param>
        public void OpenTradePanelByTradeNum(string tradeNums)
        {

            if (tradeNums != null && tradeNums.Trim().Length > 0)
            {
                String[] oidStrings = tradeNums.Trim().Split(',');
                if (oidStrings != null && oidStrings.Length > 0)
                {
                    TradeCaptureCommands.OpenTradeByTradeNumberCommand.Execute(oidStrings);
                }
            }
        }

         public void OpenTradePanelByCINNum(string aStr)
        {
            bool useExchTool = defaultICTSPreferCPNumber();
            CSAndQualifier andQualifier = new CSAndQualifier();
            List<Trade> trades = null;

            andQualifier.Qualifiers.Add(new CSKeyValueQualifier("cargo_id_number", CSQualifierOperatorSelectors.QualifierOperatorEqual, aStr));
            andQualifier.Qualifiers.Add(new CSKeyValueQualifier("trade_status_code", CSQualifierOperatorSelectors.QualifierOperatorNotEqual, "DELETE"));
            trades = ServiceUtil.FetchEntityByQualifierUsingSqlService<Trade>("Trade", andQualifier, null, null);

            if (trades == null || trades.Count == 0)
            {
                string str =  "No such CIN #: " + aStr;
                MessageBox.Show(str, null, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else if (trades != null && trades.Count > 1)
            {
                string str =  "There are " + trades.Count + " trades with " + aStr + " as the " + "CIN #. " + " Do you want to open all of them? ";
                if (MessageBox.Show(str, "TradeCapture", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
            }

            if (trades != null && trades.Count > 0)
            {
                openTradePanelForTrades(trades);
            }
        }

        public void OpenTradePanelByCPContrNum(string aStr)
        {
            bool useExchTool = defaultICTSPreferCPNumber();
            CSAndQualifier andQualifier = new CSAndQualifier();
            List<Trade> trades = null;

            andQualifier.Qualifiers.Add(new CSKeyValueQualifier("acct_ref_num", CSQualifierOperatorSelectors.QualifierOperatorEqual, aStr));
            andQualifier.Qualifiers.Add(new CSKeyValueQualifier("trade_status_code", CSQualifierOperatorSelectors.QualifierOperatorNotEqual, "DELETE"));
            trades = ServiceUtil.FetchEntityByQualifierUsingSqlService<Trade>("Trade", andQualifier, null, null);

            if(trades == null || trades.Count == 0)
            {
                string str = (useExchTool ? "Exchange Tools" : "CP. Contr.") + " #: " + aStr;
                str = "No such" + str;
                MessageBox.Show(str, null, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else if(trades != null && trades.Count >1 )
            {
                string str = (useExchTool ? "Exchange Tools" : "CP. Contr.") + " #.  ";
                str = "There are " + trades.Count + " trades with " + aStr + " as the " + str + " Do you want to open all of them? ";
                if (MessageBox.Show(str, "TradeCapture", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
            }

            if (trades != null && trades.Count > 0)
            {
                openTradePanelForTrades(trades);
            }
        }

        public void openTradePanelForTrades(List<Trade> trades)
        {
            if (trades != null && trades.Count > 0)
            {
                string oidString = null;
                foreach (Trade trade in trades)
                {
                    if (trade != null && trade.tradeNum != null)
                        oidString = oidString + trade.tradeNum.ToString() + ",";
                }

                if (oidString.EndsWith(","))
                    oidString = oidString.Remove(oidString.Length - 1);

                if (oidString != null)
                    OpenTradePanelByTradeNum(oidString);
            }
        }

        public bool defaultICTSPreferCPNumber()
        {
            bool defaultICTSPreferCPNumber = false;
            object requiredFlag = EOConfigServiceHelper.EOConfigService.ConfigdataForKey("ICTSPreferCPNumber");
            if (requiredFlag != null && (requiredFlag.ToString().StartsWith("y") || requiredFlag.ToString().StartsWith("Y")))
                defaultICTSPreferCPNumber = true;
            return defaultICTSPreferCPNumber;
        }

        public void NewAsCopyTradePanelByTradeNum(string tradeNums)
        {

            if (tradeNums != null && tradeNums.Trim().Length > 0)
            {
                String[] oidStrings = tradeNums.Trim().Split(',');
                if (oidStrings != null && oidStrings.Length > 0)
                {
                    TradeCaptureCommands.NewAsCopyTradeByTradeNumberCommand.Execute(oidStrings);
                }
            }
        }

        public void NewAsCopyStdFldsTradePanelByTradeNum(string tradeNums)
        {
            if (tradeNums != null && tradeNums.Trim().Length > 0)
            {
                String[] oidStrings = tradeNums.Trim().Split(',');
                if (oidStrings != null && oidStrings.Length > 0)
                {
                    TradeCaptureCommands.NewAsCopyStdFldsTradeByTradeNumberCommand.Execute(oidStrings);
                }
            }
        }

        private void RegisterFetchSpecifications()
        {
            userEntityService.RegisterEntityFetchSpecifications("Trade", new FutureTradeFetchSpecifications());
            userEntityService.RegisterEntityFetchSpecifications("ExchangeOptionTradeItem", new ListedOptionsTradeFetchSpecifications());
            userEntityService.RegisterEntityFetchSpecifications("StorageTradeItem", new StorageAgreementTradeFetchSpecifications());
            userEntityService.RegisterEntityFetchSpecifications("PhysicalTradeItem", new PhysicalTradeFetchSpecifications());
        }

        public bool CanAddSpecs(object panelCommandParameter)
        {
            return true;

        }

        //Venu-CLC 
        public void ShowCLCPanelData(GenericRecord genericRecord)
        {
            CreditCheckerPnlCtlNew creditCheckerPnl = new CreditCheckerPnlCtlNew(this.container);
            //creditCheckerPnl.container = this.container;

            IPopupService popupService = container.Resolve<IPopupService>();
            //creditCheckerPnl.container = this.container;
            popupService.AddPopup("Credit Limit Checker", creditCheckerPnl, false, false);
        }

        public void ShowAllLibraryFormulasData(GenericRecord genericRecord)
        {
            ShowAllLibraryFormulasCtl shwAllLibFormulas = new ShowAllLibraryFormulasCtl();
            shwAllLibFormulas.container = this.container;

            try
            {
                IDataStore dataStoreService = DataStoreServiceHelper.createNewDataStoreService();
                cs.fw.infra.service.IDataStoreService dataStore = (cs.fw.infra.service.IDataStoreService)dataStoreService;
                ICacheService cacheService = new CacheService(container, (IDataStoreService)dataStore);
                //string mergeType = GenericRecord.UserChangesMergeOverwriteUserChanges;
                //IMybatisDataAccessService MybatisDataAccessService = container.Resolve<IMybatisDataAccessService>();
                //CSFetchSpecification fetchSpecification = new CSFetchSpecification("Formula", "Tradelayout", SpecificationTypes.System);
                //CSKeyValueQualifier qualifier = new CSKeyValueQualifier("f.formula_use", CSQualifierOperatorSelectors.QualifierOperatorEqual, "L");
                //fetchSpecification.Qualifier = qualifier;
                //List<Formula> formulas = MybatisDataAccessService.FetchForQuery<Formula>(fetchSpecification, true, true, cs.fw.eo.EOUtil.GetResultMapName("Formula"), true, null, true, dataStore);
                List<Formula> formulas = TradeCaptureUtil.LibraryFormulas;
                string isFromTradeFormPanel = string.Empty;
                if (formulas == null || formulas.Count == 0)
                {
                    MessageBox.Show("Library Formulas not found!", null, MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {
                    shwAllLibFormulas.libFormulasgrid.ItemsSource = formulas;
                    //if (formulas != null && formulas.Count > 0)
                    //{
                    //    foreach (Formula f in formulas)
                    //    {
                    //        if (f != null && f.formulaUse.Equals("L"))
                    //        {
                    //            f.dataStoreService = dataStore;
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Library Formulas cannot be fethed because of the following reason.\n" + ex.Message, null, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            IPopupService popupService = container.Resolve<IPopupService>();

            //below line modified for issue 1400319
            //popupService.AddPopup("Library Formulas", shwAllLibFormulas);
            popupService.AddPopup("Library Formulas", shwAllLibFormulas, false, false);
        }

        private void ShowLibraryFormulasForLineItemData(GenericRecord record)
        {
            //Here record is generally Formula Only
            LineItemLibraryFormulasCtl lineItemLibFormulaCtl = new LineItemLibraryFormulasCtl();
            try
            {
                //List<Formula> formulas = TradeCaptureUtil.LibraryFormulas.Where(f => (f.formulaUse !=null && f.formulaUse.Equals("D"))).ToList();
                IDataStoreService dataStoreService = record.dataStoreService as IDataStoreService;
                IMybatisDataAccessService MybatisDataAccessService = container.Resolve<IMybatisDataAccessService>();
                CSFetchSpecification fetchSpecification = new CSFetchSpecification("Formula", "Tradelayout", SpecificationTypes.System);
                CSKeyValueQualifier qualifier = new CSKeyValueQualifier("f.formula_use", CSQualifierOperatorSelectors.QualifierOperatorEqual, "D");
                fetchSpecification.Qualifier = qualifier;
                List<Formula> formulas = MybatisDataAccessService.FetchForQuery<Formula>(fetchSpecification, true, true, cs.fw.eo.EOUtil.GetResultMapName("Formula"), true, null, true, dataStoreService);
                if (formulas == null || formulas.Count == 0)
                {
                    MessageBox.Show("Library Formulas not found!", null, MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {
                    lineItemLibFormulaCtl.libFormulasgrid.ItemsSource = formulas;
                    lineItemLibFormulaCtl.selectedTradeFormula = record as Formula;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Library Formulas cannot be fethed because of the following reason.\n" + ex.Message, null, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            IPopupService popupService = container.Resolve<IPopupService>();

            //below line modified for issue 1400319
            //popupService.AddPopup("Library Formulas", shwAllLibFormulas);
            popupService.AddPopup("Library Formulas", lineItemLibFormulaCtl, false, false);
        }
        public void SaveLibraryFormulaData(UserControl ctrl)
        {
            //IDataStoreService dataService = DataStoreServiceHelper.createNewDataStoreService();
            //dataService.ResetAllChanges();
            GenericRecord genericRecord = (GenericRecord)ctrl.DataContext;
            IDataStoreService dataService = (IDataStoreService)(genericRecord as Formula).dataStoreService;
            dataService.MergeGenericObject(genericRecord);
            ValidateAndSaveFormulaData(dataService, genericRecord, ctrl);
        }

        private void RemoveInternalTradeDetials(GenericRecord genericRecord, IDataStoreService dataService)
        {
            if (genericRecord is Trade)
            {

                Trade ParentTrade = (genericRecord as Trade);
                if (ParentTrade != null && ParentTrade.inhouseInd != null && ParentTrade.inhouseInd == "N")
                {
                    Trade oppTrade = ParentTrade.oppositeTrade;
                    if (ParentTrade.internalChildTrades != null)
                        ParentTrade.internalChildTrades.Clear();
                    ParentTrade.internalParentTrade = null;
                    if (oppTrade != null)
                    {
                        if (oppTrade.tradeComments != null)
                        {
                            foreach (TradeComment comment in oppTrade.tradeComments)
                            {
                                dataService.AddObjectAndRelationsToDeletedObjectsList(comment);
                            }
                            oppTrade.tradeComments = null;
                        }
                        if (oppTrade.internalChildTrades != null)
                            oppTrade.internalChildTrades.Clear();
                        oppTrade.internalParentTrade = null;
                        if (ParentTrade.oppositeTrade != null)
                        {
                            ParentTrade.oppositeTrade.inhouseInd = "N";
                            ParentTrade.oppositeTrade.acctNum = ParentTrade.acctNum;
                            ParentTrade.oppositeTrade.account = ParentTrade.account;
                        }

                        ParentTrade.oppositeTrade = null;
                        //dataService.AddObjectAndRelationsToDeletedObjectsList(oppTrade);
                    }
                }
            }
        }

        public void SaveTradeData(UserControl tradePanelCtrl)
        {
            TradePanelCtl tableCtl = null;
            if (tradePanelCtrl != null)
            {
                tableCtl = tradePanelCtrl as TradePanelCtl;
                if (tableCtl != null)
                    tableCtl.IsBeingModifiedBySystem = true;
            }
            GenericRecord genericRecord = null;
            if (tradePanelCtrl != null)
                genericRecord = (GenericRecord)tradePanelCtrl.DataContext;

            bool IsApplyChanges = false;
            Trade trade = (genericRecord as Trade);
            if (trade != null && trade.internalParentTrade != null)
            {
                trade = trade.internalParentTrade;
                if (genericRecord != null && genericRecord is Trade)
                    genericRecord = trade;
            }
            TradeOrder ato;
            if (trade != null && trade.tradeOrders != null && trade.tradeOrders.Count > 1)
            {
                for (int i = 0; i < trade.tradeOrders.Count; i++)
                {
                    ato = trade.tradeOrders[i];
                    if (ato.IsSummary && ato.IsApplyChanges)
                    {
                        string msg = "You have modified the strip template order.To apply the changes to the detail orders,You need to click the 'Update Changes to Detail Orders' button while in template mode";
                        MessageBox.Show(msg, "Trade Capture", MessageBoxButton.OK);
                        //ato.IsApplyChanges = false;
                        //IsApplyChanges = true;//ADSO-107: commenting for save the trade in details mode also
                        return;
                    }
                    if (!IsApplyChanges)
                        ato.IsApplyChanges = false;
                }
            }

            //I#ADSO-3184: only set date not the timestamp
            if (trade != null)
            {
                DateTime _aContrDate = trade.contrDate.Value;
                if (_aContrDate != null)
                    trade.contrDate = _aContrDate.Date;
            }

            string priceLineItems = null;
            bool validateLocationTankInfo = ValidateLocationTankInfo(trade.TradeItems);
            foreach (TradeItem ti in trade.TradeItems)
            {
                if (validateLocationTankInfo)
                {
                    if (ti != null && ti.tradeFormulas != null && ti.tradeFormulas.Count > 0)
                    {
                        foreach (TradeFormula tiF in ti.tradeFormulas)
                        {
                            if (tiF != null && tiF.fallBackInd != null && !tiF.fallBackInd.Equals("Y")
                                && tiF.formula != null && tiF.formula.formulaBodies != null && tiF.formula.formulaBodies.Count > 0)
                            {
                                foreach (FormulaBody fb in tiF.formula.formulaBodies)
                                {
                                    if (fb != null && fb.fbModularInfo != null && fb.fbModularInfo.riskTradingPeriod != null && fb.fbModularInfo.riskTradingPeriod.lastDelDate != null)
                                    {
                                        DateTime tradeContrDate;
                                        tradeContrDate = trade.contrDate.Value;
                                        if (fb.fbModularInfo.riskTradingPeriod.lastDelDate < tradeContrDate)
                                        {
                                            if (fb.fbModularInfo.basisCommodity != null && fb.fbModularInfo.basisCommodity.cmdtyShortName != null)
                                            {
                                                if (priceLineItems == null)
                                                    priceLineItems = fb.fbModularInfo.basisCommodity.cmdtyShortName;
                                                else
                                                    priceLineItems += ", " + fb.fbModularInfo.basisCommodity.cmdtyShortName;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (tableCtl != null)
                        tableCtl.IsBeingModifiedBySystem = false;
                    return;
                }

                List<TradeItemSpec> tiSpecs = ti.tradeItemSpecs;
                if (tiSpecs != null && tiSpecs.Count > 0)
                {
                    if (tiSpecs.Where(x => x.specification == null).Count() > 0)
                    {
                        string validationMessage = String.Format("Specification Name is not entered in order {0}, item {1}.", ti.orderNum, ti.itemNum);
                        MessageBox.Show(validationMessage, "Trade Capture", MessageBoxButton.OK, MessageBoxImage.Error);
                        if (tableCtl != null)
                            tableCtl.IsBeingModifiedBySystem = false;
                        return;
                    }
                }
            }
            if (priceLineItems != null && !priceLineItems.Equals(string.Empty))
            {
                string validationMessage = "Trading period on the price line item(s): " + priceLineItems + " were before the trade contract date.";
                MessageBox.Show(validationMessage, "Trade Capture", MessageBoxButton.OK, MessageBoxImage.Error);
                if (tableCtl != null)
                    tableCtl.IsBeingModifiedBySystem = false;
                return;
            }

            if (!IsApplyChanges)
            {
                //List<GenericRecord> dirtyObjs = new List<GenericRecord>();
                //GetAllDirtyObjects(genericRecord, dirtyObjs);

                //TODO:once datastore for each panel is implemented need to fetch the data store using the generic record.
                IDataStoreService dataService = (IDataStoreService)(genericRecord as Trade).dataStoreService;
                //IDataStoreService dataService = cmdParam.View
                RemoveInternalTradeDetials(genericRecord, dataService);
                TradeOrder tempOrder;
                int strpDetOrderCnt = 0;
                if ((genericRecord as Trade).tradeOrders != null && (genericRecord as Trade).tradeOrders.Count > 0)
                {                                                                                                                                                                                                                                              ///{
                    for (int i = 0; i < (genericRecord as Trade).tradeOrders.Count; i++)
                    {
                        tempOrder = (genericRecord as Trade).tradeOrders[i];
                        if (tempOrder.IsSummary)//|| (genericRecord as Trade).SelectedTradeItem.IsDirty))///|| [_tradeItem.tradeiteme isDirty])))
                        {
                            strpDetOrderCnt = tempOrder.stripDetailOrderCount.Value;
                            tempOrder.updateStripDetailCount(this.container);
                            if (tempOrder.isInDb && strpDetOrderCnt != tempOrder.stripDetailOrderCount.Value)
                            {
                                //If below assignment is not done, tempOrder.stripDetailOrderCount returns the updated value for the next save click.
                                tempOrder.stripDetailOrderCount = strpDetOrderCnt;
                                MessageBox.Show("You have modified the strip template order. To apply the changes to the detail orders, you need to click the 'Apply changes to Detail' button", "Warning", MessageBoxButton.OK);
                                if (tableCtl != null)
                                    tableCtl.IsBeingModifiedBySystem = false;
                                return;
                            }
                        }
                    }
                }

                bool isThereAnyWrnings = false;
                ValidationMessageHolder.Instance.RemoveFailureAndWarningValidationMessage();
                foreach (TradeItem aTradeItem in trade.TradeItems)
                {
                    if (CLOP.ntest(aTradeItem.isInDb) && CLOP.test(aTradeItem.needToValidateRiskPeriod())
                        && CLOP.ntest(aTradeItem.tradeOrder.IsDetail))
                    {
                        TradingPeriod tp = aTradeItem.riskTradingPeriod;
                        DateTime? tpStartDate = (tp != null) ? tp.firstDelDate : null;
                        DateTime? tpEndDate = (tp != null) ? tp.lastDelDate : null;
                        DateTime? delStartDate = null;
                        DateTime? delEndDate = null;
                        if (aTradeItem is WetPhysicalTradeItem && aTradeItem.tradeItemWetPhy != null)
                        {
                            delStartDate = aTradeItem.tradeItemWetPhy.delDateFrom;
                            delEndDate = aTradeItem.tradeItemWetPhy.delDateTo;
                        }
                        else if (aTradeItem is DryPhysicalTradeItem && aTradeItem.tradeItemDryPhy != null)
                        {
                            delStartDate = aTradeItem.tradeItemDryPhy.delDateFrom;
                            delEndDate = aTradeItem.tradeItemDryPhy.delDateTo;
                        }
                        else if (aTradeItem is StorageTradeItem && aTradeItem.tradeItemStorage != null)
                        {
                            delStartDate = aTradeItem.tradeItemStorage.storageStartDate;
                            delEndDate = aTradeItem.tradeItemStorage.storageEndDate;
                        }
                        else if (aTradeItem is TransportTradeItem && aTradeItem.tradeItemTransport != null)
                        {
                            delStartDate = aTradeItem.tradeItemTransport.dischDateFrom;
                            delEndDate = aTradeItem.tradeItemTransport.dischDateTo;
                        }
                        else if (aTradeItem is BunkerTradeItem && (aTradeItem as BunkerTradeItem).tradeTermInfo() != null)
                        {
                            delStartDate = (aTradeItem as BunkerTradeItem).tradeTermInfo().contrStartDate;
                            delEndDate = (aTradeItem as BunkerTradeItem).tradeTermInfo().contrEndDate;
                        }

                        if (CLOP.test(tpStartDate)
                                && CLOP.test(tpEndDate)
                                && CLOP.test(delStartDate)
                                && CLOP.test(delEndDate)
                                && ((((TimeSpan)(delEndDate - tpStartDate)).TotalDays) < 0 || ((TimeSpan)(tpEndDate - delStartDate)).TotalDays < 0))
                        {

                            String message = "The Risk Period's delivery date range (" + tpStartDate.Value.ToString("MMM dd, yyyy") + " to " + tpEndDate.Value.ToString("MMM dd, yyyy") +
                                ") does not overlap with the trade's delivery period (" + delStartDate.Value.ToString("MMM dd, yyyy") + " to " + delEndDate.Value.ToString("MMM dd, yyyy") +
                                "), in order " + aTradeItem.orderNum + ", item " + aTradeItem.itemNum + ".";

                            //vrs.addValidationResult(new ValidationResult("Risk Period", message, "Failure", false, null, null, null));
                            ValidationMessageHolder.Instance.AddValidationMessage("WARNING: Risk Period", message, Severity.Warning);
                            if (!isThereAnyWrnings)
                                isThereAnyWrnings = true;
                        }

                    }
                    //I#ADSO-3621: validation logic moved from server to client side for "The Fixed Price Quantity and the positive Floating Price Quantity are not the same in swap order".
                    if (aTradeItem != null && aTradeItem is CashPhysicalTradeItem)
                    {
                        Formula formula = aTradeItem.mainformula;
                        if (aTradeItem.IsFixedVersusFloat() && formula != null)
                        {
                            List<FormulaBody> floatQuotes = formula.quoteBodies();
                            if (floatQuotes.Count > 1)
                            {
                                decimal? cQty = aTradeItem.contrQty;
                                decimal? totalFloatQty = 0;
                                decimal? aQty;
                                foreach (FormulaBody body in floatQuotes)
                                {
                                    aQty = body.formulaQtyPcntVal;
                                    if (aQty > 0)
                                    {
                                        if (body.formulaQtyUomCode.Equals("%"))
                                            totalFloatQty += cQty * aQty / 100;
                                        else
                                        {
                                            decimal? uomFactor = null;
                                            if (!aTradeItem.contractQtyUom.Equals(body.formulaQtyUom))
                                            {
                                                uomFactor = UomConversion.factorFromToForCommodity(body.formulaQtyUom, aTradeItem.contractQtyUom, body.formulaComponents[0].commodityMarket.commodity);
                                            }
                                            if (uomFactor != null && uomFactor > 0)
                                                totalFloatQty += aQty * uomFactor;
                                            else
                                                totalFloatQty += aQty;
                                        }
                                    }
                                }
                                if (Math.Abs((double)(cQty - totalFloatQty)) > 0.01)
                                {
                                    isThereAnyWrnings = true;
                                    String message = "The Fixed Price Quantity and the positive Floating Price Quantity are not the same in swap order " + aTradeItem.orderNum + ".";
                                    ValidationMessageHolder.Instance.AddValidationMessage("WARNING: Inconsistent Fixed and Floating Quantities", message, Severity.Warning);
                                }
                            }
                        }
                    }
                }
                if (isThereAnyWrnings)
                {
                    if (MessageBox.Show("Some warnings have been found in this trade.\nDo you want to stop to review ?", "TradeCapture", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        return;
                }

                GetChildDataViaIBatis(genericRecord as Trade);
                genericRecord = TradeItem.FillChildTradeItemData(genericRecord);
                OtcOptionTradeBusiness.FinishEditingOfTrade(genericRecord);
                genericRecord = TradeItem.FormulaBodyParseString(genericRecord);
                FutureTradeBusiness.UpdateTotalFillQtyIfFillExists(genericRecord);
                SwapTradeBusiness.UpdateContractQtyForItems(genericRecord);
                TradeItemBusiness.SetDateTypeValuesOnEntityTags(genericRecord as Trade);
                ValidateAndSaveData(dataService, genericRecord);
            }
            if (tableCtl != null)
                tableCtl.IsBeingModifiedBySystem = false;
        }

        string showTankWarnForPhysicalTrades = null;
        string ShowTankWarnForPhysicalTrades()
        {
            if (showTankWarnForPhysicalTrades == null)
            {
                showTankWarnForPhysicalTrades = "yes";
                string val = CSInfraUtil.ClientConfigUtil.getConfigString("ShowTankWarnForPhysicalTrades");
                if (val != null && val.ToLower() == "no")
                    showTankWarnForPhysicalTrades = "no";
            }
            return showTankWarnForPhysicalTrades;
        }

        private bool ValidateLocationTankInfo(List<TradeItem> tradeItems)
        {
            string validationMsg = "The Tank Number (Miscellaneous Panel) is not entered for following items : ";
            string itemMsg = "";
            if (tradeItems != null)
            {
                foreach (TradeItem ti in tradeItems)
                {
                    if (ti is WetPhysicalTradeItem)
                    {
                       if (ShowTankWarnForPhysicalTrades() == "yes")
                       {
                            if (ti.tradeItemWetPhy != null && ti.tradeItemWetPhy.mot != null &&
                                ti.tradeItemWetPhy.mot.motTypeCode.Equals("S") && ti.tradeItemWetPhy.locationTankInfo == null)
                            {
                                if (itemMsg == "")
                                    itemMsg = itemMsg + "order : " + ti.orderNum + ", item : " + ti.itemNum;
                                else
                                    itemMsg = itemMsg + "\n" + "order : " + ti.orderNum + ", item : " + ti.itemNum;
                            }
                        }
                    }
                    else if (ti is StorageTradeItem)
                    {
                        if (ti.tradeItemStorage != null && ti.tradeItemStorage.mot != null && ti.tradeItemStorage.locationTankInfo == null)
                        {
                            if (itemMsg == "")
                                itemMsg = itemMsg + "order : " + ti.orderNum + ", item : " + ti.itemNum;
                            else
                                itemMsg = itemMsg + "\n" + "order : " + ti.orderNum + ", item : " + ti.itemNum;
                        }
                    }
                }
            }

            if (itemMsg != "")
            {
                validationMsg = validationMsg + "\n" + itemMsg + "\n" + "Do You Want To Proceed?";
                if (MessageBox.Show(validationMsg, "Tank Number Not Entered", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void GetChildDataViaIBatis(Trade trade)
        {
            if (trade.isInDb)
            {
                foreach (TradeItem ti in trade.TradeItems)
                {
                    if (!ti.isChildDataFetched && ti.isInDb)
                    {
                        ti.getDetailDataViaIBatis(container);
                    }
                }
            }
        }
        //Moved Business logic to FutureTradeBusiness class file.
        /*
        private void UpdateTotalFillQtyIfFillExists(GenericRecord genericRecord)
        {
            if (genericRecord is Trade)
            {
                Trade trade = genericRecord as Trade;
                if (trade != null && trade.tradeOrders != null && trade.tradeOrders.Count > 0)
                {
                    foreach (TradeOrder order in trade.tradeOrders)
                    {
                        if (order.tradeItems != null && order.tradeItems.Count > 0)
                        {
                            foreach (TradeItem item in order.tradeItems)
                            {
                                if (item is FutureTradeItem)
                                {
                                    if ((item as FutureTradeItem).tradeItemFut != null)
                                    {
                                        FutureTradeBusiness.UpdateTotalFillQtyAndAveFillPrice(item as FutureTradeItem);
                                        //updateFutTotalFillQty(item as FutureTradeItem);                                        
                                    }
                                }
                                else if (item is ExchangeOptionTradeItem)
                                {
                                    if ((item as ExchangeOptionTradeItem).tradeItemExchOpt != null)
                                    {
                                        updateTotalFillQty(item as ExchangeOptionTradeItem);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }        

        public void updateTotalFillQty(ExchangeOptionTradeItem exchTItem)
        {
            decimal? total_FillQty = decimal.Zero;
            if (exchTItem != null && exchTItem.tradeItemExchOpt.fills != null)
            {
                foreach (TradeItemFill tfill in exchTItem.tradeItemExchOpt.fills)
                {
                    total_FillQty += tfill.fillQty;
                }
                exchTItem.tradeItemExchOpt.totalFillQty = total_FillQty;
            }
        }

        public void updateFutTotalFillQty(FutureTradeItem futItem)
        {
            decimal? total_FillQty = decimal.Zero;
            if (futItem != null && futItem.tradeItemFut.fills != null)
            {
                foreach (TradeItemFill tfill in futItem.tradeItemFut.fills)
                {
                    total_FillQty += tfill.fillQty;
                }
                futItem.tradeItemFut.totalFillQty = total_FillQty;
            }
        }*/

        private void ValidateAndSaveFormulaData(IDataStoreService dataService, GenericRecord genericRecord, UserControl ctrl)
        {
            //TODO need to add validation here.
            ViewState viewState = dataService.CreateViewState();

            List<GenericMessage> listOfMessages = new List<GenericMessage>();
            SaveFormula saveFormula = new SaveFormula();
            if (genericRecord.isInDb == false)
                saveFormula.FormulaRecordKey = genericRecord.RecordKey;
            else
            {
                saveFormula.FormulaNum = (genericRecord as Formula).formulaNum;
            }

            listOfMessages.Add(saveFormula);
            ProcessSaveFromulaChanges(listOfMessages, viewState, dataService, genericRecord, ctrl);

        }

        private void ValidateAndSaveScenarioData(IDataStoreService dataService, GenericRecord genericRecord)
        {
            //TODO need to add validation here.
            ViewState viewState = dataService.CreateViewState();

            List<GenericMessage> listOfMessages = new List<GenericMessage>();
            SaveScenario saveScenario = new SaveScenario();
            listOfMessages.Add(saveScenario);
            ProcessSaveScenarioChanges(listOfMessages, viewState, dataService, genericRecord);

        }

        private void ProcessSaveFromulaChanges(List<GenericMessage> listOfMessages, ViewState viewState, IDataStoreService dataService, GenericRecord genericRecord, UserControl ctrl)
        {
            string jBossMachineName = null;
            string failurMsg = string.Empty;
            string sucessMsgHeader = string.Empty;
            StringBuilder sb = new StringBuilder();
            string successMsg = string.Empty;
            bool isExisted = genericRecord.isInDb;

            CompositeMessage message = new CompositeMessage();
            message.Messages = listOfMessages;
            IMessageProcessorService processService = container.Resolve<IMessageProcessorService>();
            CompositeResponseMessage response = null;
            response = processService.ProcessSaveFormula(message, viewState, dataService);
            UserControl userCtrl = null;
            if (ctrl is LibraryFormulaSaveCtl)
            {
                userCtrl = (ctrl as LibraryFormulaSaveCtl).getFormulaEditor() as UserControl;
            }
            else
                userCtrl = ctrl;
            if (userCtrl is ComplexFormulaEditor && (userCtrl as ComplexFormulaEditor).errorMessageGrpBox.IsVisible)
            {
                (userCtrl as ComplexFormulaEditor).txtErrMessage.Text = "";
                (userCtrl as ComplexFormulaEditor).errorMessageGrpBox.Visibility = Visibility.Collapsed;
            }
            else if (userCtrl is ModularFormulaEditor && (userCtrl as ModularFormulaEditor).errorMessageGrpBox.IsVisible)
            {
                (userCtrl as ModularFormulaEditor).txtErrMessage.Text = "";
                (userCtrl as ModularFormulaEditor).errorMessageGrpBox.Visibility = Visibility.Collapsed;
            }
            else if (userCtrl is ModularLineItemEditor && (userCtrl as ModularLineItemEditor).errorMessageGrpBox.IsVisible)
            {
                (userCtrl as ModularLineItemEditor).txtErrMessage.Text = "";
                (userCtrl as ModularLineItemEditor).errorMessageGrpBox.Visibility = Visibility.Collapsed;
            }

            foreach (ResponseMessage resposeMsg in response.responseMessages)
            {
                GenericMessage sentMessage = message[resposeMsg.correlationId];
                if (sentMessage != null)
                {
                    if (resposeMsg.statusMessage.Contains("Failure"))
                    {
                        jBossMachineName = resposeMsg.machineName;
                        failurMsg = "Records Not Saved" + System.Environment.NewLine;
                        sb.Append(resposeMsg.statusMessage).Append(Environment.NewLine);

                        if (resposeMsg.HasStaleObjects())
                        {
                            failurMsg += buildStaleObjectsErrorMessage(resposeMsg.staleObjectsRecordKey);
                            //single line of code commented and the next line aded by Padma Rao on 19 Apr 2012 based on issue 1344189.
                            /*
                             * The message is modified based on teh mail sent by Gwen on 18 Apr 2012 
                             */
                            //failurMsg += "Do you want to Refresh the objects ?";
                            failurMsg += "Would you like to refresh the objects and Save?";
                            MessageBoxResult result = MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                foreach (string recordKey in resposeMsg.staleObjectsRecordKey)
                                {
                                    EntityRecord localGenericRecord = dataService.GetGenericObject(recordKey) as EntityRecord;
                                    //condition added by Padma Rao oN 24 Sep 2012 based on issue 1356105
                                    /*
                                     * condition added based on the rolling logs for gwoody in cs3 env on sep 12 2012
                                     */
                                    if (localGenericRecord != null)
                                    {
                                        //ServiceUtil.FetchStaleObject<EntityRecord>(localGenericRecord);
                                        ServiceUtil.FetchStaleObject<EntityRecord>(localGenericRecord, dataService);
                                        int? transId = null;

                                        try
                                        {
                                            transId = Convert.ToInt32(localGenericRecord.GetType().GetProperty("transId").GetValue(localGenericRecord, null));

                                        }
                                        catch (Exception e)
                                        {
                                        }

                                        string logmsg = "StaleObject -> " + recordKey + " New TransId : " + transId;
                                    }
                                }
                                ValidateAndSaveFormulaData(dataService, genericRecord, ctrl);
                                return;
                            }
                        }
                        else
                        {
                            if (resposeMsg.validationResults != null)
                            {
                                failurMsg = string.Empty;
                                //looping through all the validation results and adding to the display message
                                foreach (tc.bean.validation.ValidationResult vr in response.responseMessages[0].validationResults.results)
                                {
                                    failurMsg += vr.ToString() + System.Environment.NewLine;
                                }
                                if (userCtrl is ComplexFormulaEditor)
                                {
                                    (userCtrl as ComplexFormulaEditor).errorMessageGrpBox.Visibility = Visibility.Visible;
                                    (userCtrl as ComplexFormulaEditor).txtErrMessage.Text = "Changes not saved \n" + failurMsg;
                                    (userCtrl as ComplexFormulaEditor).txtErrMessage.Focus();


                                }
                                else if (userCtrl is ModularFormulaEditor)
                                {
                                    (userCtrl as ModularFormulaEditor).errorMessageGrpBox.Visibility = Visibility.Visible;
                                    (userCtrl as ModularFormulaEditor).txtErrMessage.Text = "Changes not saved \n" + failurMsg;
                                    (userCtrl as ModularFormulaEditor).txtErrMessage.Focus();

                                }
                                else if (userCtrl is ModularLineItemEditor)
                                {
                                    (userCtrl as ModularLineItemEditor).errorMessageGrpBox.Visibility = Visibility.Visible;
                                    (userCtrl as ModularLineItemEditor).txtErrMessage.Text = "Changes not saved \n" + failurMsg;
                                    (userCtrl as ModularLineItemEditor).txtErrMessage.Focus();

                                }
                                else
                                    ValidationMessageHolder.Instance.AddValidationMessage("Changes not saved", failurMsg, Severity.Failure);


                                //MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.OK);
                            }
                        }
                    }//end of failure block
                    else if (resposeMsg.statusMessage.Contains("Success"))
                    {
                        if (resposeMsg.validationResults.results != null)
                        {
                            foreach (tc.bean.validation.ValidationResult vr in resposeMsg.validationResults.results)
                            {
                                successMsg += vr.ToString() + System.Environment.NewLine;
                            }
                            string msg = null;
                            string forlumasList = null;

                            //if (genericRecord is Trade && (genericRecord as Trade).inhouseInd  != null 
                            //    && (genericRecord as Trade).inhouseInd.Equals("I"))
                            //{
                            if (resposeMsg.affectedObjectsRecordKey != null && resposeMsg.affectedObjectsRecordKey.Count > 0)
                            {
                                foreach (String recKey in resposeMsg.affectedObjectsRecordKey)
                                {
                                    if (recKey.Contains("Formula:"))
                                    {
                                        if (forlumasList == null)
                                            forlumasList = recKey.Substring(recKey.IndexOf('=') + 1).Replace(';', ' ');
                                        else
                                            forlumasList += "/ " + recKey.Substring(recKey.IndexOf('=') + 1).Replace(';', ' ');
                                    }
                                    GenericRecord gr = dataService.GetGenericObject(recKey);
                                    if (gr != null)
                                        gr.AcceptChanges();
                                }
                                msg = "Formulas# " + forlumasList + " saved successfully";
                                sucessMsgHeader = "Formula(s) saved successfully";
                            }
                            //}
                            else
                            {
                                if (isExisted)
                                {
                                    msg = "Formula# " + (genericRecord as Formula).formulaNum + " updated successfully";
                                    sucessMsgHeader = "Formula updated successfully";
                                }
                                else
                                {
                                    msg = "Formula# " + (genericRecord as Formula).formulaNum + " saved successfully";
                                    sucessMsgHeader = "Formula saved successfully";
                                }
                            }
                            ValidationMessageHolder.Instance.ClearAndAddNewValidationMessage(sucessMsgHeader, msg, Severity.Sucess);
                            //if (MessageBox.Show(msg, "Trade Capture", MessageBoxButton.OK) == MessageBoxResult.OK)
                            //{

                            //TradeCaptureUtil.LibraryFormulas.Add(genericRecord as Formula);
                            int index = TradeCaptureUtil.GetIndexOfLibraryFormula(genericRecord as Formula);
                            TradeCaptureUtil.AddToLibraryFormulas(genericRecord as Formula, index);
                            if (ctrl is ILibraryFormulaSaveCtl)
                            {
                                if (((ILibraryFormulaSaveCtl)ctrl).getFormulaEditor() != null)
                                {
                                    Window editorWindow = Window.GetWindow(((ILibraryFormulaSaveCtl)ctrl).getFormulaEditor());

                                    Window parentWindow = Window.GetWindow(ctrl);
                                    if (parentWindow != null)
                                    {
                                        parentWindow.Close();
                                    }
                                    if (editorWindow != null)
                                    {
                                        editorWindow.Close();
                                    }
                                }
                            }



                            //}
                            //if (_panelService != null && _panelService.DockSites != null && _panelService.DockSites.Count > 0)
                            //{
                            //    DockWindow dw = (_panelService.DockSites[0] as DockSite).ActiveDockWindow;
                            //    string tradeRecordKey = "Formula#: " + forlumasList.TrimEnd();
                            //    storeUserResourcesForRecentTrades(forlumasList.TrimEnd());
                            //    if (dw == null)
                            //    {
                            //        foreach (DockWindow dwindow in (_panelService.DockSites[0] as DockSite).GetDockWindows())
                            //        {
                            //            if ((dwindow.Header.ToString().Contains(tradeRecordKey) || dwindow.Header.ToString().Contains("Trade#: New")) && dwindow.IsSelected)
                            //            {
                            //                dwindow.Header = tradeRecordKey;
                            //                dwindow.Close();
                            //                dataService.ResetAllChanges();
                            //                (dataService as DataStoreService)._panelMappingDictionary.Remove(tradeRecordKey);
                            //                break;
                            //            }
                            //        }
                            //    }
                            //    else
                            //    {
                            //        dw.Header = genericRecord.RecordKey;
                            //        dw.Close();
                            dataService.ResetAllChanges();
                            //        (dataService as DataStoreService)._panelMappingDictionary.Remove(tradeRecordKey);
                            //    }
                            //}
                            newFormulaOptions = null;
                        }
                    }
                }
            }

        }

        private void ProcessSaveScenarioChanges(List<GenericMessage> listOfMessages, ViewState viewState, IDataStoreService dataService, GenericRecord genericRecord)
        {
            string jBossMachineName = null;
            string failurMsg = string.Empty;
            StringBuilder sb = new StringBuilder();
            string successMsg = string.Empty;

            CompositeMessage message = new CompositeMessage();
            message.Messages = listOfMessages;
            IMessageProcessorService processService = container.Resolve<IMessageProcessorService>();
            CompositeResponseMessage response = null;
            response = processService.ProcessSaveScenario(message, viewState, dataService);

            foreach (ResponseMessage resposeMsg in response.responseMessages)
            {
                GenericMessage sentMessage = message[resposeMsg.correlationId];
                if (sentMessage != null)
                {
                    if (resposeMsg.statusMessage.Contains("Failure"))
                    {
                        jBossMachineName = resposeMsg.machineName;
                        failurMsg = "Records Not Saved" + System.Environment.NewLine;
                        sb.Append(resposeMsg.statusMessage).Append(Environment.NewLine);

                        if (resposeMsg.HasStaleObjects())
                        {
                            failurMsg += buildStaleObjectsErrorMessage(resposeMsg.staleObjectsRecordKey);
                            //single line of code commented and the next line aded by Padma Rao on 19 Apr 2012 based on issue 1344189.
                            /*
                             * The message is modified based on teh mail sent by Gwen on 18 Apr 2012 
                             */
                            //failurMsg += "Do you want to Refresh the objects ?";
                            failurMsg += "Would you like to refresh the objects and Save?";
                            MessageBoxResult result = MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                foreach (string recordKey in resposeMsg.staleObjectsRecordKey)
                                {
                                    EntityRecord localGenericRecord = dataService.GetGenericObject(recordKey) as EntityRecord;
                                    //condition added by Padma Rao oN 24 Sep 2012 based on issue 1356105
                                    /*
                                     * condition added based on the rolling logs for gwoody in cs3 env on sep 12 2012
                                     */
                                    if (localGenericRecord != null)
                                    {
                                        //ServiceUtil.FetchStaleObject<EntityRecord>(localGenericRecord);
                                        ServiceUtil.FetchStaleObject<EntityRecord>(localGenericRecord, dataService);
                                        int? transId = null;

                                        try
                                        {
                                            transId = Convert.ToInt32(localGenericRecord.GetType().GetProperty("transId").GetValue(localGenericRecord, null));

                                        }
                                        catch (Exception e)
                                        {
                                        }

                                        string logmsg = "StaleObject -> " + recordKey + " New TransId : " + transId;
                                    }
                                }
                                ValidateAndSaveScenarioData(dataService, genericRecord);
                                return;
                            }
                        }
                        else
                        {
                            if (resposeMsg.validationResults != null)
                            {
                                failurMsg = string.Empty;
                                //looping through all the validation results and adding to the display message
                                foreach (tc.bean.validation.ValidationResult vr in response.responseMessages[0].validationResults.results)
                                {
                                    failurMsg += vr.ToString() + System.Environment.NewLine;
                                }

                                MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.OK);
                            }
                        }
                    }//end of failure block
                    else if (resposeMsg.statusMessage.Contains("Success"))
                    {
                        if (resposeMsg.validationResults.results != null)
                        {
                            foreach (tc.bean.validation.ValidationResult vr in resposeMsg.validationResults.results)
                            {
                                successMsg += vr.ToString() + System.Environment.NewLine;
                            }
                            string msg = null;
                            string ScenariosList = null;

                            //if (genericRecord is Trade && (genericRecord as Trade).inhouseInd  != null 
                            //    && (genericRecord as Trade).inhouseInd.Equals("I"))
                            //{
                            if (resposeMsg.affectedObjectsRecordKey != null && resposeMsg.affectedObjectsRecordKey.Count > 0)
                            {
                                foreach (String recKey in resposeMsg.affectedObjectsRecordKey)
                                {
                                    if (recKey.Contains("Scenario:"))
                                    {
                                        if (ScenariosList == null)
                                            ScenariosList = recKey.Substring(recKey.IndexOf('=') + 1).Replace(';', ' ');
                                        else
                                            ScenariosList += "/ " + recKey.Substring(recKey.IndexOf('=') + 1).Replace(';', ' ');
                                    }
                                    GenericRecord gr = dataService.GetGenericObject(recKey);
                                    if (gr != null)
                                        gr.AcceptChanges();
                                }
                                //msg = "Scenario# " + ScenariosList + " saved successfully";
                                msg = "Sucessfully saved all the changes to database.";

                            }

                            //}
                            else
                            {
                                if (genericRecord != null && genericRecord is Scenario)
                                    msg = "Formulas# " + (genericRecord as Scenario).oid + " saved successfully";
                            }
                            //MessageBox.Show(msg, "Trade Capture", MessageBoxButton.OK);
                            ValidationMessageHolder.Instance.AddValidationMessage("Changes are saved", msg, Severity.Sucess);
                            //if (_panelService != null && _panelService.DockSites != null && _panelService.DockSites.Count > 0)
                            //{
                            //    DockWindow dw = (_panelService.DockSites[0] as DockSite).ActiveDockWindow;
                            //    string tradeRecordKey = "Formula#: " + forlumasList.TrimEnd();
                            //    storeUserResourcesForRecentTrades(forlumasList.TrimEnd());
                            //    if (dw == null)
                            //    {
                            //        foreach (DockWindow dwindow in (_panelService.DockSites[0] as DockSite).GetDockWindows())
                            //        {
                            //            if ((dwindow.Header.ToString().Contains(tradeRecordKey) || dwindow.Header.ToString().Contains("Trade#: New")) && dwindow.IsSelected)
                            //            {
                            //                dwindow.Header = tradeRecordKey;
                            //                dwindow.Close();
                            //                dataService.ResetAllChanges();
                            //                (dataService as DataStoreService)._panelMappingDictionary.Remove(tradeRecordKey);
                            //                break;
                            //            }
                            //        }
                            //    }
                            //    else
                            //    {
                            //        dw.Header = genericRecord.RecordKey;
                            //        dw.Close();
                            ScenarioUtil.scenariosLst = null;
                            ScenarioUtil.FetchAllScenarios(dataService);
                            dataService.ResetAllChanges();
                            //        (dataService as DataStoreService)._panelMappingDictionary.Remove(tradeRecordKey);
                            //    }
                            //}
                        }
                    }
                }
            }

        }


        private void ProcessSaveCreateLiveScenarioChanges(List<GenericMessage> listOfMessages, ViewState viewState, IDataStoreService dataService, GenericRecord genericRecord, UserControl userCtl)
        {
            string jBossMachineName = null;
            string failurMsg = string.Empty;
            StringBuilder sb = new StringBuilder();
            string successMsg = string.Empty;

            CompositeMessage message = new CompositeMessage();
            message.Messages = listOfMessages;
            IMessageProcessorService processService = container.Resolve<IMessageProcessorService>();
            CompositeResponseMessage response = null;
            response = processService.ProcessCreateLiveScenario(message, viewState, dataService);

            foreach (ResponseMessage resposeMsg in response.responseMessages)
            {
                GenericMessage sentMessage = message[resposeMsg.correlationId];
                if (sentMessage != null)
                {
                    if (resposeMsg.statusMessage.Contains("Failure"))
                    {
                        jBossMachineName = resposeMsg.machineName;
                        failurMsg = "Records Not Saved" + System.Environment.NewLine;
                        sb.Append(resposeMsg.statusMessage).Append(Environment.NewLine);

                        if (resposeMsg.HasStaleObjects())
                        {
                            failurMsg += buildStaleObjectsErrorMessage(resposeMsg.staleObjectsRecordKey);
                            //single line of code commented and the next line aded by Padma Rao on 19 Apr 2012 based on issue 1344189.
                            /*
                             * The message is modified based on teh mail sent by Gwen on 18 Apr 2012 
                             */
                            //failurMsg += "Do you want to Refresh the objects ?";
                            failurMsg += "Would you like to refresh the objects and Save?";
                            MessageBoxResult result = MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                foreach (string recordKey in resposeMsg.staleObjectsRecordKey)
                                {
                                    EntityRecord localGenericRecord = dataService.GetGenericObject(recordKey) as EntityRecord;
                                    //condition added by Padma Rao oN 24 Sep 2012 based on issue 1356105
                                    /*
                                     * condition added based on the rolling logs for gwoody in cs3 env on sep 12 2012
                                     */
                                    if (localGenericRecord != null)
                                    {
                                        //ServiceUtil.FetchStaleObject<EntityRecord>(localGenericRecord);
                                        ServiceUtil.FetchStaleObject<EntityRecord>(localGenericRecord, dataService);
                                        int? transId = null;

                                        try
                                        {
                                            transId = Convert.ToInt32(localGenericRecord.GetType().GetProperty("transId").GetValue(localGenericRecord, null));

                                        }
                                        catch (Exception e)
                                        {
                                        }

                                        string logmsg = "StaleObject -> " + recordKey + " New TransId : " + transId;
                                    }
                                }
                                ValidateAndCreateLiveScenario(dataService, genericRecord, userCtl);
                                return;
                            }
                        }
                        else
                        {
                            if (resposeMsg.validationResults != null)
                            {
                                failurMsg = string.Empty;
                                //looping through all the validation results and adding to the display message
                                foreach (tc.bean.validation.ValidationResult vr in response.responseMessages[0].validationResults.results)
                                {
                                    //failurMsg += vr.ToString() + System.Environment.NewLine;
                                    ValidationMessageHolder.Instance.AddValidationMessage(vr.validationName, vr.validationMessage, Severity.Failure);
                                }

                                //MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.OK);
                            }
                        }
                    }//end of failure block
                    else if (resposeMsg.statusMessage.Contains("Success"))
                    {
                        int oid = 0;
                        if (resposeMsg.validationResults.results != null)
                        {

                            foreach (tc.bean.validation.ValidationResult vr in resposeMsg.validationResults.results)
                            {
                                if (vr.ToString().Contains("Data Saved  with oid"))
                                {
                                    oid = int.Parse(vr.ToString().Replace("Data Saved  with oid", "").Trim());
                                }
                                successMsg += vr.ToString() + System.Environment.NewLine;
                            }
                            string msg = null;
                            string ScenariosList = null;

                            //if (genericRecord is Trade && (genericRecord as Trade).inhouseInd  != null 
                            //    && (genericRecord as Trade).inhouseInd.Equals("I"))
                            //{
                            if (resposeMsg.affectedObjectsRecordKey != null && resposeMsg.affectedObjectsRecordKey.Count > 0)
                            {
                                foreach (String recKey in resposeMsg.affectedObjectsRecordKey)
                                {
                                    if (recKey.Contains("LiveScenario:"))
                                    {
                                        if (ScenariosList == null)
                                            ScenariosList = recKey.Substring(recKey.IndexOf('=') + 1).Replace(';', ' ');
                                        else
                                            ScenariosList += "/ " + recKey.Substring(recKey.IndexOf('=') + 1).Replace(';', ' ');
                                    }
                                    GenericRecord gr = dataService.GetGenericObject(recKey);
                                    if (gr != null)
                                        gr.AcceptChanges();
                                }
                                msg = "LiveScenario# " + ScenariosList + " saved successfully";
                            }

                            //}
                            else
                            {
                                if (genericRecord != null && genericRecord is LiveScenario)
                                    msg = "LiveScenarios# " + (genericRecord as LiveScenario).oid + " saved successfully";

                            }
                            if (msg == null && successMsg != null)
                                msg = successMsg;
                            if (userCtl is ScenarioInstanceCtl)
                            {
                                (userCtl as ScenarioInstanceCtl).Scenarioid = oid;
                            }
                            //  MessageBox.Show(msg, "Trade Capture", MessageBoxButton.OK);
                            ValidationMessageHolder.Instance.AddValidationMessage(msg, msg, Severity.Sucess);
                            //if (_panelService != null && _panelService.DockSites != null && _panelService.DockSites.Count > 0)
                            //{
                            //    DockWindow dw = (_panelService.DockSites[0] as DockSite).ActiveDockWindow;
                            //    string tradeRecordKey = "Formula#: " + forlumasList.TrimEnd();
                            //    storeUserResourcesForRecentTrades(forlumasList.TrimEnd());
                            //    if (dw == null)
                            //    {
                            //        foreach (DockWindow dwindow in (_panelService.DockSites[0] as DockSite).GetDockWindows())
                            //        {
                            //            if ((dwindow.Header.ToString().Contains(tradeRecordKey) || dwindow.Header.ToString().Contains("Trade#: New")) && dwindow.IsSelected)
                            //            {
                            //                dwindow.Header = tradeRecordKey;
                            //                dwindow.Close();
                            //                dataService.ResetAllChanges();
                            //                (dataService as DataStoreService)._panelMappingDictionary.Remove(tradeRecordKey);
                            //                break;
                            //            }
                            //        }
                            //    }
                            //    else
                            //    {
                            //        dw.Header = genericRecord.RecordKey;
                            //        dw.Close();
                            ScenarioUtil.scenariosLst = null;
                            ScenarioUtil.FetchAllScenarios(dataService);
                            dataService.ResetAllChanges();
                            //        (dataService as DataStoreService)._panelMappingDictionary.Remove(tradeRecordKey);
                            //    }
                            //}
                        }
                    }
                }
            }
        }

        private void ValidateAndSaveData(IDataStoreService dataService, GenericRecord genericRecord)
        {
            ValidateAndSaveData(dataService, genericRecord, 1);
        }

        private void ValidateAndSaveData(IDataStoreService dataService, GenericRecord genericRecord, int attemptno)
        {
            //dotNet side Validations
            ValidationMessageHolder.Instance.RemoveFailureAndWarningValidationMessage();
            if (genericRecord is Trade)
            {
                (genericRecord as Trade).ValidateForSave();
                if (ValidationMessageHolder.Instance.ValidationMessages.Count() > 0)
                {
                    return;
                }

                Trade _trade = genericRecord as Trade;
                if (_trade != null && _trade.inhouseInd.Equals("Y") && _trade.oppositeTrade != null)
                {
                    (dataService as DataStoreService).AddObjectAndRelationsToDeletedObjectsList(_trade.oppositeTrade);
                    _trade.oppositeTrade = null;
                    _trade.internalChildTrades = null;
                }
                if (_trade != null && _trade.TradeItems != null && _trade.TradeItems.Count > 0)
                {
                    bool temp = false;
                    foreach (TradeItem ti in _trade.TradeItems)
                    {
                        List<TradeFormula> tfList = ti.tradeFormulas;
                        if (tfList != null && tfList.Count > 0)
                        {
                            foreach (TradeFormula tf in tfList)
                            {
                                if ((tf.formula.formulaType != null && tf.formula.formulaType != FormulaProperty.FP_EVENT_TYPE) && 
                                    (tf.formula.eventPriceTermProp != null || (tf.formula.eventPriceTerms != null && tf.formula.eventPriceTerms.Count > 0)))
                                {
                                    (tf.formula.tradeFormula[0].tradeItem.trade.dataStoreService as cs.fw.infra.service.IDataStoreService).RemoveObjectAndRelations(tf.formula.eventPriceTermProp);
                                    tf.formula.RemoveFromEventPriceTerms(tf.formula.eventPriceTermProp);
                                }
                            }
                        }
                        if (ti.pSInd == null)
                        {
                            string failurMsg = "Buy/Sell is not Selected for Order " + ti.orderNum.Value.ToString() + " item " + ti.itemNum.Value.ToString();
                            ValidationMessageHolder.Instance.AddValidationMessage("Buy/Sell is not Selected", failurMsg, Severity.Failure);
                            temp = true;
                        }
                    }
                    if (temp == true)
                        return;
                }
            }
            //End code changes

            ViewState viewState = dataService.CreateViewState();
            List<GenericMessage> listOfMessages = new List<GenericMessage>();
            SaveTrade saveTrade = new SaveTrade();

            if (genericRecord.isInDb == false)
                saveTrade.TradeRecordKey = genericRecord.RecordKey;
            else
            {
                if ((genericRecord as Trade).internalParentTradeNum != null)
                    saveTrade.TradeNum = (genericRecord as Trade).oppositeTrade.tradeNum;
                else
                    saveTrade.TradeNum = (genericRecord as Trade).tradeNum;
                (genericRecord as Trade).tradeModDate = DateTime.Now;
                ISecurityService securityService = container.Resolve<ISecurityService>();
                com.tc.frameworks.ictseos.eoaccount.IctsUser user = securityService.LoggedInUser as com.tc.frameworks.ictseos.eoaccount.IctsUser;
                (genericRecord as Trade).tradeModInit = securityService.LoggedInUserInit;
            }

            listOfMessages.Add(saveTrade);
            ProcessSaveChanges(listOfMessages, viewState, dataService, genericRecord, attemptno);
        }

        private void ProcessSaveChanges(List<GenericMessage> listOfMessages, ViewState viewState,
            IDataStoreService dataService, GenericRecord genericRecord, int attemptno)
        {
            string jBossMachineName = null;
            string failurMsg = string.Empty;
            StringBuilder sb = new StringBuilder();
            string successMsg = string.Empty;

            CompositeMessage message = new CompositeMessage();
            message.Messages = listOfMessages;

            IMessageProcessorService processService = container.Resolve<IMessageProcessorService>();
            CompositeResponseMessage response = null;

            //bool isNew = !genericRecord.isInDb;
            bool updatedContrInfoOnly = genericRecord.isInDb && (genericRecord as Trade).isContrInfoFieldUpdated();

            ValidationMessageHolder.Instance.RemoveFailureAndWarningValidationMessage();
            if (System.Windows.Forms.Cursor.Current == System.Windows.Forms.Cursors.Default)
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            response = processService.ProcessSaveTrade(message, viewState, dataService);
            if (System.Windows.Forms.Cursor.Current == System.Windows.Forms.Cursors.WaitCursor)
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;

            foreach (ResponseMessage resposeMsg in response.responseMessages)
            {
                GenericMessage sentMessage = message[resposeMsg.correlationId];
                if (sentMessage != null)
                {
                    if (resposeMsg.statusMessage.Contains("Failure"))
                    {
                        jBossMachineName = resposeMsg.machineName;
                        failurMsg = "Records Not Saved" + System.Environment.NewLine;
                        sb.Append(resposeMsg.statusMessage).Append(Environment.NewLine);

                        if (resposeMsg.HasStaleObjects())
                        {
                            // failurMsg += buildStaleObjectsErrorMessage(resposeMsg.staleObjectsRecordKey);
                            //single line of code commented and the next line aded by Padma Rao on 19 Apr 2012 based on issue 1344189.
                            /*
                             * The message is modified based on teh mail sent by Gwen on 18 Apr 2012 
                             */
                            //failurMsg += "Do you want to Refresh the objects ?";
                            //failurMsg += "Would you like to refresh the objects and Save?";
                            // MessageBoxResult result = MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.YesNo);
                            // if (result == MessageBoxResult.Yes)
                            // {
                            foreach (string recordKey in resposeMsg.staleObjectsRecordKey)
                            {
                                bool isFetchedFromDefaultDataStore = false;
                                EntityRecord localGenericRecord = dataService.GetGenericObject(recordKey) as EntityRecord;
                                if (localGenericRecord == null)
                                {
                                    localGenericRecord = dataService.defaultDataStoreService.GetGenericObject(recordKey) as EntityRecord;
                                    isFetchedFromDefaultDataStore = true;
                                }
                                //condition added by Padma Rao oN 24 Sep 2012 based on issue 1356105
                                /*
                                 * condition added based on the rolling logs for gwoody in cs3 env on sep 12 2012
                                 */
                                if (localGenericRecord != null)
                                {
                                    //ServiceUtil.FetchStaleObject<EntityRecord>(localGenericRecord);
                                    if (isFetchedFromDefaultDataStore == false)
                                        ServiceUtil.FetchStaleObject<EntityRecord>(localGenericRecord, dataService);
                                    else
                                        ServiceUtil.FetchStaleObject<EntityRecord>(localGenericRecord, dataService.defaultDataStoreService);
                                    int? transId = null;

                                    try
                                    {
                                        transId = Convert.ToInt32(localGenericRecord.GetType().GetProperty("transId").GetValue(localGenericRecord, null));

                                    }
                                    catch (Exception e)
                                    {
                                    }

                                    string logmsg = "StaleObject -> " + recordKey + " New TransId : " + transId;
                                }
                            }

                            if (attemptno == 1)
                            {
                                failurMsg = "Data was modified by another process/user, an automatic save with refreshed data is being attempted";
                                ValidationMessageHolder.Instance.AddValidationMessage(failurMsg, failurMsg, Severity.Failure);
                            }

                            if (attemptno <= 5)
                            {
                                attemptno++;
                                ValidateAndSaveData(dataService, genericRecord, attemptno);
                            }
                            else
                            {
                                failurMsg = "Data was modified by another process/user, an automatic save with refreshed data was attempted, Please reopen and save";
                                //ValidationMessageHolder.Instance.AddValidationMessage(failurMsg, failurMsg, Severity.Failure);
                                MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.OK);
                            }
                            return;

                            //}
                        }
                        else
                        {
                            if (resposeMsg.validationResults != null)
                            {
                                failurMsg = string.Empty;
                                bool staleObjectValidation = false;
                                //looping through all the validation results and adding to the display message
                                foreach (tc.bean.validation.ValidationResult vr in response.responseMessages[0].validationResults.results)
                                {
                                    if (vr.validationMessage.Contains("Row was updated or deleted by another transaction"))
                                    {
                                        staleObjectValidation = true;
                                        break;
                                    }
                                    ValidationMessageHolder.Instance.AddValidationMessage(vr.validationName, vr.validationMessage, Severity.Failure);
                                    //failurMsg += vr.ToString() + System.Environment.NewLine;
                                }

                                if (staleObjectValidation)
                                {
                                    if (attemptno == 1)
                                    {
                                        failurMsg = "Data was modified by another process/user, an automatic save with refreshed data is being attempted";
                                        ValidationMessageHolder.Instance.AddValidationMessage(failurMsg, failurMsg, Severity.Failure);
                                    }

                                    if (attemptno <= 5)
                                    {
                                        attemptno++;
                                        ValidateAndSaveData(dataService, genericRecord, attemptno);
                                    }
                                    else
                                    {
                                        failurMsg = "Data was modified by another process/user, an automatic save with refreshed data was attempted, Please reopen and save";
                                        //ValidationMessageHolder.Instance.AddValidationMessage(failurMsg, failurMsg, Severity.Failure);
                                        MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.OK);
                                    }
                                }

                                //MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.OK);
                            }
                        }
                    }//end of failure block
                    else if (resposeMsg.statusMessage.Contains("Success"))
                    {
                        if (resposeMsg.validationResults.results != null)
                        {
                            foreach (tc.bean.validation.ValidationResult vr in resposeMsg.validationResults.results)
                            {
                                successMsg += vr.ToString() + System.Environment.NewLine;
                            }
                            string msg = null;
                            string tradesList = null;
                            // [I#1396574] one line code added by Kiran Kumar P on 2nd July 2014
                            bool updateMsg = false;
                            List<string> tradesStrArray = new List<string>();

                            //if (genericRecord is Trade && (genericRecord as Trade).inhouseInd  != null 
                            //    && (genericRecord as Trade).inhouseInd.Equals("I"))
                            //{
                            if (resposeMsg.affectedObjectsRecordKey != null && resposeMsg.affectedObjectsRecordKey.Count > 0)
                            {
                                int tempTradeNum = 0;
                                foreach (String recKey in resposeMsg.affectedObjectsRecordKey)
                                {
                                    if (recKey.Contains("Trade:"))
                                    {
                                        if (tradesList == null)
                                        {
                                            tradesList = recKey.Substring(recKey.IndexOf('=') + 1).Replace(';', ' ');
                                            tempTradeNum = int.Parse(recKey.Substring(recKey.IndexOf('=') + 1).Replace(';', ' ').TrimEnd());
                                        }
                                        else
                                        {
                                            if (int.Parse(recKey.Substring(recKey.IndexOf('=') + 1).Replace(';', ' ')) > tempTradeNum)
                                            {
                                                tradesList += "/ " + recKey.Substring(recKey.IndexOf('=') + 1).Replace(';', ' ');
                                                tempTradeNum = int.Parse(recKey.Substring(recKey.IndexOf('=') + 1).Replace(';', ' ').TrimEnd());
                                            }
                                            else
                                                tradesList = recKey.Substring(recKey.IndexOf('=') + 1).Replace(';', ' ') + "/ " + tradesList;
                                        }
                                        tradesStrArray.Add("Trade#: " + recKey.Substring(recKey.IndexOf('=') + 1).Replace(';', ' ').TrimEnd());
                                    }
                                    GenericRecord gr = dataService.GetGenericObject(recKey);
                                    if (gr != null)
                                    {
                                        //gr.AcceptChanges();
                                        // [I#1396574] one line code added by Kiran Kumar P on 2nd July 2014
                                        updateMsg = true;
                                    }
                                }

                                if (updatedContrInfoOnly && updateMsg)
                                {
                                    msg = "Contract info (Analyst, Status, CP/Broker Ref # or Financing Bank) in Trade #" + tradesList +
                                        " successfully updated.";
                                    MessageBox.Show(msg, "Trade Capture", MessageBoxButton.OK);
                                }
                                else
                                {
                                    // [I#1396574] condition code added by Kiran Kumar P on 2nd July 2014
                                    if (updateMsg == true)
                                        msg = " updated";
                                    else
                                        msg = " added";

                                    msg = "Trade #" + tradesList + msg + " successfully";

                                    if (msg != null && msg.Length > 0)
                                        ValidationMessageHolder.Instance.AddValidationMessage(msg, msg, Severity.Sucess);
                                }
                            }

                            if (_panelService != null && _panelService.DockSites != null && _panelService.DockSites.Count > 0)
                            {
                                DockWindow dw = (_panelService.DockSites[0] as DockSite).ActiveDockWindow;
                                //string tradeRecordKey = "Trade#: " + tradesList.TrimEnd();
                                BackgroundWorker _worker = new BackgroundWorker();
                                _worker.WorkerReportsProgress = false;
                                _worker.WorkerSupportsCancellation = true;
                                _worker.DoWork += new DoWorkEventHandler(BackgroundInit_DoWorkToStoreUserResources);
                                _worker.RunWorkerAsync(new object[] { tradesList.TrimEnd() + " - " + (updateMsg ? "Updated" : "Added") });

                                //storeUserResourcesForRecentTrades(tradesList.TrimEnd());

                                if (dw == null)
                                {
                                    foreach (DockWindow dwindow in (_panelService.DockSites[0] as DockSite).GetDockWindows())
                                    {
                                        foreach (string tradeRecordKey in tradesStrArray)
                                        {
                                            if ((dwindow.Header.ToString().Contains(tradeRecordKey) || dwindow.Header.ToString().Contains("Trade#: New")) && dwindow.IsSelected)
                                            {
                                                dwindow.Header = tradeRecordKey;
                                                dwindow.Close();
                                                //dataService.ResetAllChanges();
                                                (dataService as DataStoreService)._panelMappingDictionary.Remove(tradeRecordKey);
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (string tradeRecordKey in tradesStrArray)
                                    {
                                        dw.Header = tradeRecordKey;// genericRecord.RecordKey;
                                        dw.Close();
                                        // dataService.ResetAllChanges();
                                        (dataService as DataStoreService)._panelMappingDictionary.Remove(tradeRecordKey);
                                    }
                                }

                            }
                        }
                    }
                }
            }

        }

        private void BackgroundInit_DoWorkToStoreUserResources(object s, DoWorkEventArgs args)
        {
            string str = (args.Argument as object[])[0] as string;
            storeUserResourcesForRecentTrades(str);
        }

        private void storeUserResourcesForRecentTrades(string tradeNum)
        {
            string resultStr = "#" + tradeNum;
            IUserPreferenceService PrefService = container.Resolve<IUserPreferenceService>();
            PrefService.SaveUserResourceForRecentTrades(resultStr);
        }

        private static string buildStaleObjectsErrorMessage(List<string> staleObjectsRecordKey)
        {
            //Local variable declaration
            string failurMsg = string.Empty;
            string temRecordKey = string.Empty;
            foreach (string recordKey in staleObjectsRecordKey)
            {
                temRecordKey = recordKey;
                temRecordKey = temRecordKey.Replace(":", " ");
                temRecordKey = temRecordKey.Replace(";", " ");
                //single line of code commented and the next line aded by Padma Rao on 19 Apr 2012 based on issue 1344189.
                /*
                 * The message is modified based on teh mail sent by Gwen on 18 Apr 2012 
                 */
                //failurMsg += temRecordKey + "was modified by another user" + System.Environment.NewLine;
                failurMsg += temRecordKey + "was modified by another process" + System.Environment.NewLine;
            }
            return failurMsg;
        }

        public void AddItemToTrade(UserControl treeViewCtl)
        {

            TreeView treeview = (treeViewCtl as TradeOrderListsCtl).ordersTreeView;
            GenericRecord selectedItem = treeview.SelectedItem as GenericRecord;

            if (selectedItem is Trade)
            {
                TradeOrdertypesCtl ordersControl = new TradeOrdertypesCtl();
                ordersControl.Trade = selectedItem as Trade;
                ordersControl.treeViewCtl = treeViewCtl;
                IPopupService popupService = container.Resolve<IPopupService>();
                popupService.AddPopup("Symphony", false, ordersControl);
            }
            else if (selectedItem is TradeOrder)
            {
                AddOrderOrItemToTrade(treeViewCtl, null);
            }

        }




        private void AddOrderOrItemToTrade(UserControl treeViewCtl, TradeOrderOptionsSelected orderOption)
        {
            TreeView treeview = (treeViewCtl as TradeOrderListsCtl).ordersTreeView;
            GenericRecord selectedItem = treeview.SelectedItem as GenericRecord;
            IDataStoreService dataStoreService = null;
            if (selectedItem is Trade)
            {
                TradeOrder newOrder = null;
                newOrder = GetNewTradeOrder(selectedItem as Trade, orderOption);
                selectedItem.IsSelected = false;
                newOrder.IsSelected = true;
                treeViewCtl.DataContext = selectedItem as Trade;
                //treeview.Items.Refresh();
                //treeview.UpdateLayout();
                dataStoreService = (IDataStoreService)selectedItem.dataStoreService;
            }
            else if (selectedItem is TradeOrder)
            {
                TradeItem newTradeItem = null;
                TradeItem ti = (selectedItem as TradeOrder).tradeItems[0];
                dataStoreService = (IDataStoreService)ti.trade.dataStoreService;
                if (ti is FutureTradeItem)
                {
                    newTradeItem = FutureTradeBusiness.AddNewTradeItem(selectedItem as TradeOrder);
                }
                else if (ti is ExchangeOptionTradeItem)
                {
                    newTradeItem = ExchangeOptionTradeBusiness.AddNewTradeItem(selectedItem as TradeOrder);
                }
                else if (ti is StorageTradeItem)
                {
                    TradeItem childTi = StorageAgreementTradeBusiness.AddNewTradeItem(selectedItem as TradeOrder, container);
                    newTradeItem = childTi.parentItem;
                    dataStoreService.MergeGenericObject(newTradeItem);
                    dataStoreService.MergeGenericObject(childTi);

                }
                else if (ti is TransportTradeItem)
                {
                    TradeItem childTi = TransportAgreementBusiness.AddNewTradeItem(selectedItem as TradeOrder);
                    newTradeItem = childTi.parentItem;
                    dataStoreService.MergeGenericObject(newTradeItem);
                    dataStoreService.MergeGenericObject(childTi);
                }
                // There is no Add Item functionality for OTC Trades
                //else if (ti is OtcOptionTradeItem)
                //{
                //    TradeItem childTi = OtcOptionTradeBusiness.AddNewTradeItem(selectedItem as TradeOrder);
                //    newTradeItem = childTi.parentItem;
                //    dataStoreService.MergeGenericObject(newTradeItem);
                //    dataStoreService.MergeGenericObject(childTi);
                //}
                else if ((selectedItem as TradeOrder).orderTypeCode.Equals("PHYSBUNK"))
                {
                    TradeItem childTi = PhysicalTradesBusiness.AddNewBunkerTradeItem(selectedItem as TradeOrder);
                    newTradeItem = childTi.parentItem;
                    dataStoreService.MergeGenericObject(newTradeItem);
                    dataStoreService.MergeGenericObject(childTi);
                }
                else
                {
                    MessageBox.Show("Can not add trade Item to this trade order.");
                }

                if (newTradeItem != null)
                {
                    newTradeItem.useMktFormulaForPl = "Y";
                    selectedItem.IsSelected = false;
                    newTradeItem.IsSelected = true;
                }
                treeViewCtl.DataContext = (selectedItem as TradeOrder).trade;
                treeview.Items.Refresh();
                treeview.UpdateLayout();
            }
            //IDataStoreService dataStoreService = container.Resolve<IDataStoreService>();
            dataStoreService.MergeGenericObject(selectedItem);
        }

        private TradeOrder GetNewTradeOrder(Trade trade, TradeOrderOptionsSelected orderOption)
        {
            TradeOrder newOrder = null;
            GetChildDataViaIBatis(trade);
            switch (orderOption.OrderType)
            {
                case "SWAP":
                    newOrder = SwapTradeBusiness.AddNewTradeOrder(trade, orderOption);
                    break;
                case "SWAPFLT":
                    newOrder = SwapTradeBusiness.AddNewTradeOrder(trade, orderOption);
                    break;
                case "FUTURE":
                    newOrder = FutureTradeBusiness.AddNewTradeOrder(trade);
                    break;
                case "EXCHGOPT":
                    newOrder = ExchangeOptionTradeBusiness.AddNewTradeOrder(trade);
                    break;
                case "STORAGE":
                    newOrder = StorageAgreementTradeBusiness.AddNewTradeOrder(trade);
                    break;
                case "BARGE":
                    newOrder = TransportAgreementBusiness.AddNewTradeOrder(trade, orderOption);
                    break;
                case "TRUCK":
                    newOrder = TransportAgreementBusiness.AddNewTradeOrder(trade, orderOption);
                    break;
                case "PIPELINE":
                    newOrder = TransportAgreementBusiness.AddNewTradeOrder(trade, orderOption);
                    break;
                case "VLCC":
                    newOrder = TransportAgreementBusiness.AddNewTradeOrder(trade, orderOption);
                    break;
                case "RAILCAR":
                    newOrder = TransportAgreementBusiness.AddNewTradeOrder(trade, orderOption);
                    break;
                case "OTCCASH":
                    newOrder = OtcOptionTradeBusiness.AddNewTradeOrder(trade, orderOption);
                    break;
                case "OTCAPO":
                    newOrder = OtcOptionTradeBusiness.AddNewTradeOrder(trade, orderOption);
                    break;
                case "OTCPHYS":
                    newOrder = OtcOptionTradeBusiness.AddNewTradeOrder(trade, orderOption);
                    break;
                case "PHYSICAL":
                    newOrder = PhysicalTradesBusiness.AddNewTradeOrder(trade, orderOption);
                    break;
                case "PHYSBUNK":
                    newOrder = PhysicalTradesBusiness.AddNewTradeOrder(trade, orderOption);
                    break;
                case "PHYSCONC":
                    newOrder = PhysicalTradesBusiness.AddNewTradeOrder(trade, orderOption);
                    break;
                case "EFPEXCH":
                    newOrder = PhysicalTradesBusiness.AddNewTradeOrder(trade, orderOption);
                    break;
                case "PARTIAL":
                    newOrder = PhysicalTradesBusiness.AddNewTradeOrder(trade, orderOption);
                    break;
                case "OTCPHYS_SWAPFLT":
                    newOrder = OtcOptionTradeBusiness.AddOtcOptionPhysicallySettledSwapTradeOrder(trade, orderOption, "SWAPFLT");
                    break;
                case "OTCPHYS_SWAP":
                    newOrder = OtcOptionTradeBusiness.AddOtcOptionPhysicallySettledSwapTradeOrder(trade, orderOption, "SWAP");
                    break;
                case "OTCPHYS_PHYSICAL":
                    newOrder = OtcOptionTradeBusiness.AddOtcOptionPhysicallySettledTradeOrder(trade, orderOption, "PHYSICAL");
                    break;
                case "OTCPHYS_PARTIAL":
                    newOrder = OtcOptionTradeBusiness.AddOtcOptionPhysicallySettledTradeOrder(trade, orderOption, "PARTIAL");
                    break;
                case "CURRENCY":
                    newOrder = CurrencyTradeBusiness.AddNewTradeOrder(trade);
                    break;

            }
            trade.NotifyPropertyChanged("displayTradeOrders");
            newOrder.NotifyPropertyChanged("displayTradeItems");
            return newOrder;

        }

        public void DeleteItemFromTrade(UserControl treeViewCtl)
        {
            TreeView treeview = (treeViewCtl as TradeOrderListsCtl).ordersTreeView;
            GenericRecord selectedItem = treeview.SelectedItem as GenericRecord;

            IDataStoreService dataStoreService = null;
            if (selectedItem is Trade)
            {
                string msg = "Do you want to delete the Trade ";
                if (selectedItem.isInDb == true)
                    msg += ": " + (selectedItem as Trade).tradeNum.Value;
                msg += "?";
                MessageBoxResult result = MessageBox.Show(msg, "Trade Capture", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    dataStoreService = (IDataStoreService)selectedItem.dataStoreService;
                    ProcessDeleteTrade(selectedItem as Trade, dataStoreService);
                }
            }
            else if (selectedItem is TradeOrder)
            {
                TradeOrder to = (selectedItem as TradeOrder);
                TradeOrder internOrder = null;
                TradeOrder internChild = null;

                if(to.trade.isInternal && (to.trade.internalParentTradeNum == null || to.trade.internalParentTrade == null))
                {
                    internOrder = to.internalChildOrder();
                    if(internOrder != null)
                        internChild = internOrder.underlyingOrder();
                }
                //Code added to check any trade item is scheduled on this selected trade order at deletion time[Ref. OC]
                if (to != null && !to.IsSummary)
                {
                    if (to.IsAnyItemAcheduled() || (to.underlyingOrder() != null && to.underlyingOrder().IsAnyItemAcheduled()))
                    {
                        if (to.orderTypeCode.Equals("PHYSB2BC"))
                            MessageBox.Show("You cannot delete the selected B2B Contract because there are sale trades existing against this Purchase Contract .", "Trade Capture", MessageBoxButton.OK);
                        else
                            MessageBox.Show("You cannot delete the selected order because some of its items have been scheduled.", "Trade Capture", MessageBoxButton.OK);
                        return;
                    }
                }
                if (to.IsAnyCostVouched() || (to.underlyingOrder() != null && to.underlyingOrder().IsAnyCostVouched()) || (internOrder != null && internOrder.IsAnyCostVouched()) || (internChild != null && internChild.IsAnyCostVouched()))
                {
                    MessageBox.Show("You cannot delete the selected order because some of its costs have been vouched.", "Trade Capture", MessageBoxButton.OK);
                    return;
                }
                string msg = "Do you want to delete the Trade Order :";
                if (to.tradeNum.HasValue)
                    msg += to.tradeNum.Value + "/";
                msg += to.orderNum + " ?";
                MessageBoxResult result = MessageBox.Show(msg, "Trade Capture", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Trade trade = to.trade;

                    //TODO : Implement same behavior as OC TC App when there is only one TO
                    //       Currently we are not doing anything if Trade has only one TO and deleting that order.
                    if (trade != null && trade.tradeOrders.Count > 0)
                    {
                        if (to.orderTypeCode.Equals("OTCPHYS") || to.orderTypeCode.Equals("SWAP") || to.orderTypeCode.Equals("SWAPFLT") || to.orderTypeCode.Equals("PHYSICAL") || to.orderTypeCode.Equals("PARTIAL"))
                        {
                            TradeDefaultController.newInstance().removeObjectsForTradeOrder(to);
                            TradeOrder _tradeOrder = to.deleteParentOrChildTradeOrder(trade, to);
                            if (_tradeOrder != null)
                            {
                                trade.RemoveFromTradeOrders(_tradeOrder);
                                dataStoreService = (IDataStoreService)trade.dataStoreService;
                                if (to.isInDb)
                                    dataStoreService.AddToDeletedObjectsList(_tradeOrder);
                                else
                                    dataStoreService.RemoveObjectAndRelations(_tradeOrder);
                            }
                        }

                        /* trade.RemoveFromTradeOrders(to);
                         dataStoreService = (IDataStoreService)trade.dataStoreService;
                         if (to.isInDb == false && to.tradeItems != null && to.tradeItems.Count > 0)
                         {
                             foreach (TradeItem ti in to.tradeItems)
                             {
                                 ti.RemoveChildData(dataStoreService);
                             }
                         }
                         dataStoreService = (IDataStoreService)trade.dataStoreService;
                         if (to.isInDb)
                             dataStoreService.AddToDeletedObjectsList(to);
                         else
                             dataStoreService.RemoveObjectAndRelations(to);*/

                        // New Logic Added below

                        List<TradeOrder> listOfTORemoved = new List<TradeOrder>();
                        listOfTORemoved.Add(to);
                        if (to.IsSummary && trade != null && trade.tradeOrders != null && trade.tradeOrders.Count > 0)
                        {
                            TradeOrder ato = null;
                            int tradeOrderCount = trade.tradeOrders.Count;
                            for (int i = 0; i < tradeOrderCount; i++)
                            {
                                ato = trade.tradeOrders[i];
                                if ((ato != null && to.orderStripNum != null && to.orderStripNum.Value > 0 && to.orderStripNum == ato.orderStripNum))
                                {
                                    if (listOfTORemoved != null && !listOfTORemoved.Contains(ato))
                                        listOfTORemoved.Add(ato);
                                }
                            }
                        }
                        for (int i = 0; i < listOfTORemoved.Count; i++)
                        {
                            TradeOrder ato = listOfTORemoved[i];
                            trade.RemoveFromTradeOrders(ato);
                            dataStoreService = (IDataStoreService)trade.dataStoreService;
                            if (ato.isInDb == false && ato.tradeItems != null && ato.tradeItems.Count > 0)
                            {
                                foreach (TradeItem ti in ato.tradeItems)
                                {
                                    ti.RemoveChildData(dataStoreService);
                                }
                            }
                            dataStoreService = (IDataStoreService)trade.dataStoreService;
                            if (ato.isInDb)
                                dataStoreService.AddToDeletedObjectsList(ato);
                            else
                                dataStoreService.RemoveObjectAndRelations(ato);
                        } // End of Code

                        if (trade != null && trade.tradeOrders != null && trade.tradeOrders.Count > 0)
                        {
                            //when the trade order or item is deleted we need to regenerate the strip detail orders
                            //so we also update hasDirty on the main/prelim/market/risk formula on the template order
                            //because the copyChangeToItemExcludingOverridenFields method looks for hasDirty property to be true
                            TradeOrder ato = null;
                            TradeItem ati = null;
                            TradeFormula aTF = null;
                            Formula aFormula = null;
                            for (int i = 0; i < trade.tradeOrders.Count; i++)
                            {
                                ato = trade.tradeOrders[i];
                                if (ato != null && ato.IsSummary)
                                {
                                    ati = ato.tradeItems[0]; //we only look at the first item here 
                                    //TODO - loop thru all items if summary has more than 1 item
                                    for (int j = 0; j < ati.tradeFormulas.Count; j++)
                                    {
                                        aTF = ati.tradeFormulas[j];
                                        if (aTF != null)
                                        {
                                            aFormula = aTF.formula; //this may be main/prelim/risk/market formula
                                            if (aFormula != null && !aFormula.hasDirty)
                                            {
                                                aFormula.hasDirty = true;
                                                aFormula.NotifyPropertyChanged("hasDirty");
                                                //ADSO-3305 - For Order deletion, the IsApplyChanges should be False which actually becomes True in above line
                                                aFormula.tradeFormula[0].tradeItem.tradeOrder.IsApplyChanges = false;
                                                aFormula.tradeFormula[0].tradeItem.tradeOrder.NotifyPropertyChanged("IsApplyChanges");
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
                        }

                        //dataStoreService.AddObjectAndRelationsToDeletedObjectsList(to);
                        treeViewCtl.DataContext = (selectedItem as TradeOrder).trade;
                        treeview.Items.Refresh();
                        treeview.UpdateLayout();
                    }
                }
            }
            else if (selectedItem is TradeItem)
            {
                TradeItem ti = (selectedItem as TradeItem);
                //Code added to check selected trade item is scheduled or not at deletion time [Ref. OC]
                if (ti != null && ti.tradeOrder != null && !ti.tradeOrder.IsSummary)
                {
                    if (ti.isAIScheduled() || (ti.childTradeItem != null && ti.childTradeItem.isAIScheduled()))
                    {
                        if (ti.tradeOrder.orderTypeCode.Equals("PHYSB2BC"))
                            MessageBox.Show("You cannot delete the selected B2B Contract because there are sale trades existing against this Purchase Contract.", "Trade Capture", MessageBoxButton.OK);
                        else
                            MessageBox.Show("Trade Deletion : You cannot delete this trade because some of its orders have been scheduled, exercised.", "Trade Capture", MessageBoxButton.OK);
                        return;
                    }
                }
                if (ti.tradeOrder.orderTypeCode == "EFPEXCH")
                {
                    string strmsg = "You cannot delete the last Physical or Future Item in an EFP Order";
                    MessageBox.Show(strmsg, "Trade Capture");
                    return;
                }
                if(ti.isAnyCostVouched || (ti.InternalOppositeItem() != null && ti.InternalOppositeItem().isAnyCostVouched))
                {
                    MessageBox.Show("You cannot delete the selected item because some of its costs have been vouched.", "Trade Capture", MessageBoxButton.OK);
                    return;
                }

                string msg = "Do you want to delete the Trade Item :";
                if (ti.tradeNum.HasValue)
                    msg += ti.tradeNum.Value + "/";
                msg += ti.orderNum + "/" + ti.itemNum + " ?";
                MessageBoxResult result = MessageBox.Show(msg, "Trade Capture", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    bool canDeleteTOrder = false;
                    TradeOrder to = ti.tradeOrder;
                    Trade trade = (selectedItem as TradeItem).trade;
                    dataStoreService = (IDataStoreService)trade.dataStoreService;
                    if (to != null)
                    {
                        if (selectedItem is TransportTradeItem || selectedItem is StorageTradeItem)
                        {
                            TradeItem oppTI = null;
                            if (selectedItem is TransportTradeItem)
                                oppTI = (selectedItem as TransportTradeItem).OppositeSideTradeItem;
                            else
                                oppTI = (selectedItem as StorageTradeItem).OppositeSideTradeItem;
                            to.RemoveFromTradeItems(oppTI);
                            //oppTI.tradeOrder = null;
                            trade.TradeItems.Remove(oppTI);
                            if (oppTI.isInDb == false)
                                oppTI.RemoveChildData(dataStoreService);
                            dataStoreService.AddObjectAndRelationsToDeletedObjectsList(oppTI);
                        } //Added by Brahma Reddy on 14 Feb 2014.
                        else if (selectedItem is FutureTradeItem)
                        {
                            FutureTradeItem futItem = selectedItem as FutureTradeItem;
                            if (futItem.trade.IsInternal() && futItem.trade.isInDb)
                            {
                                TradeItem childTradeItem = FutureTradeBusiness.GetInternalChildItem(trade.oppositeTrade, futItem);
                                if (childTradeItem != null)
                                {
                                    childTradeItem.tradeOrder.RemoveFromTradeItems(childTradeItem);
                                    //childTradeItem.tradeOrder = null;
                                    trade.oppositeTrade.TradeItems.Remove(childTradeItem);
                                    //dataStoreService = (IDataStoreService)trade.dataStoreService;
                                    if (childTradeItem.isInDb == false)
                                        childTradeItem.RemoveChildData(dataStoreService);
                                    dataStoreService.AddObjectAndRelationsToDeletedObjectsList(childTradeItem);
                                }
                            }
                        }
                        else if (selectedItem is OtcOptionTradeItem)
                        {
                            OtcOptionTradeItem otcOptionTradeItem = selectedItem as OtcOptionTradeItem;
                            if (otcOptionTradeItem.trade.IsInternal() && otcOptionTradeItem.trade.isInDb)
                            {
                                TradeItem childTradeItem = OtcOptionTradeBusiness.GetInternalChildItem(trade.oppositeTrade, otcOptionTradeItem);
                                if (childTradeItem != null)
                                {
                                    childTradeItem.tradeOrder.RemoveFromTradeItems(childTradeItem);
                                    trade.oppositeTrade.TradeItems.Remove(childTradeItem);
                                    //if (childTradeItem.TiDistributions != null && childTradeItem.TiDistributions.Count > 0)
                                    //{
                                    //    foreach (TradeItemDist tid in childTradeItem.TiDistributions)
                                    //    {
                                    //        if (tid.accumNum != null && tid.accumNum > 0)
                                    //        {             

                                    //        }
                                    //    }
                                    //}
                                    if (childTradeItem.isInDb == false)
                                        childTradeItem.RemoveChildData(dataStoreService);
                                    dataStoreService.AddObjectAndRelationsToDeletedObjectsList(childTradeItem);
                                }
                            }
                        }
                        //End of code.
                        to.RemoveFromTradeItems(ti);
                        if (ti.isInDb == false)
                            ti.RemoveChildData(dataStoreService);
                        //ADSO-964 - below code is not needed as this is throwing nested null exceptions when tradeItem.tradeOrder is called; 
                        //which inturn causing many issues while applyChanges
                        //ti.tradeOrder = null;
                        if (to != null && to.IsDirty)
                        {
                            TradeOrder ato = null;
                            TradeItem ati = null;
                            Formula aFormula = null;
                            TradeFormula aTF = null;

                            trade.hasDirty = true;
                            trade.NotifyPropertyChanged("hasDirty");

                            //when the trade order or item is deleted we need to regenerate the strip detail orders
                            //so we also update hasDirty on the main/prelim/market/risk formula on the template order
                            //because the copyChangeToItemExcludingOverridenFields method looks for hasDirty property to be true
                            for (int i = 0; i < trade.tradeOrders.Count; i++)
                            {
                                ato = trade.tradeOrders[i];
                                if (ato != null && ato.IsSummary && ato.tradeItems.Count > 0)
                                {
                                    ati = ato.tradeItems[0]; //we only look at the first item here 
                                    //TODO - loop thru all items if summary has more than 1 item
                                    for (int j = 0; j < ati.tradeFormulas.Count; j++)
                                    {
                                        aTF = ati.tradeFormulas[j];
                                        if (aTF != null)
                                        {
                                            aFormula = aTF.formula; //this may be main/prelim/risk/market formula
                                            if (aFormula != null && !aFormula.hasDirty)
                                            {
                                                aFormula.hasDirty = true;
                                                aFormula.NotifyPropertyChanged("hasDirty");
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
                        }

                        //ti.tradeOrder = null; //ADSO-79 commenting this logic to avoid nested transaction exception at server side 
                        //while deleting tradeitem from physical strip trade
                        if (to.tradeItems.Count == 0)
                        {
                            //trade.RemoveFromTradeOrders(to);
                            //canDeleteTOrder = true;
                            // Added   below Code
                            List<TradeOrder> listOfTORemoved = new List<TradeOrder>();
                            listOfTORemoved.Add(to);
                            if (to.IsSummary && trade != null && trade.tradeOrders != null && trade.tradeOrders.Count > 0)
                            {
                                TradeOrder ato = null;
                                int tradeOrderCount = trade.tradeOrders.Count;
                                for (int i = 0; i < tradeOrderCount; i++)
                                {
                                    ato = trade.tradeOrders[i];
                                    if (ato != null && to.orderStripNum != null && to.orderStripNum.Value > 0 && to.orderStripNum == ato.orderStripNum)
                                    {
                                        if (listOfTORemoved != null && !listOfTORemoved.Contains(ato))
                                            listOfTORemoved.Add(ato);
                                    }
                                }
                            }
                            for (int i = 0; i < listOfTORemoved.Count; i++)
                            {
                                TradeOrder ato = listOfTORemoved[i];
                                trade.RemoveFromTradeOrders(ato);
                                dataStoreService = (IDataStoreService)trade.dataStoreService;
                                if (ato.isInDb == false && ato.tradeItems != null && ato.tradeItems.Count > 0)
                                {
                                    foreach (TradeItem tim in ato.tradeItems)
                                    {
                                        tim.RemoveChildData(dataStoreService);
                                    }
                                }
                                dataStoreService = (IDataStoreService)trade.dataStoreService;
                                if (ato.isInDb)
                                    dataStoreService.AddToDeletedObjectsList(ato);
                                else
                                    dataStoreService.RemoveObjectAndRelations(ato);
                            } // End of Code
                        }
                        //ti.tradeOrder = null;
                        trade.TradeItems.Remove(ti);
                    }
                    trade.SelectedTradeItem = null;
                    trade.NotifyPropertyChanged("SelectedTradeItem");
                    //dataStoreService = (IDataStoreService)trade.dataStoreService;
                    //if (!canDeleteTOrder)
                    //{
                    //    dataStoreService.AddObjectAndRelationsToDeletedObjectsList(ti);
                    //}
                    //else
                    //{
                    //    dataStoreService.AddObjectAndRelationsToDeletedObjectsList(to);
                    //}

                    if (ti.isInDb)
                        dataStoreService.AddToDeletedObjectsList(ti);
                    else
                        dataStoreService.RemoveObjectAndRelations(ti);
                    /*if (canDeleteTOrder)
                    {
                        if (to.isInDb)
                            dataStoreService.AddToDeletedObjectsList(to);
                        else
                            dataStoreService.RemoveObjectAndRelations(to);
                    }*/
                    treeViewCtl.DataContext = trade;
                    treeview.Items.Refresh();
                    treeview.UpdateLayout();
                }
            }
        }

        private void DeleteLibraryFormulaData(UserControl formualCtl)
        {

            Formula formula = formualCtl.DataContext as Formula;// (Formula)gRec;
            bool isFormualDeleted = false;
            IDataStoreService dataStoreService = null;
            dataStoreService = (IDataStoreService)formula.dataStoreService;
            if (formula.isInDb == false)
            {
                try
                {

                    MessageBox.Show("Formula deleted.", "Trade Capture");
                    isFormualDeleted = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Formula deleted : " + ex.StackTrace.ToString());
                    isFormualDeleted = false;
                }
            }
            else
            {
                List<GenericMessage> listOfMessages = new List<GenericMessage>();
                DeleteFormula deleteFormula = new DeleteFormula();
                deleteFormula.FormulaNum = formula.formulaNum.Value;
                deleteFormula.FormulaRecordKey = formula.RecordKey;
                listOfMessages.Add(deleteFormula);

                CompositeMessage message = new CompositeMessage();
                message.Messages = listOfMessages;
                IMessageProcessorService processService = container.Resolve<IMessageProcessorService>();
                CompositeResponseMessage response = null;
                response = processService.ProcessDeleteFormula(message, dataStoreService);

                string jBossMachineName = null;
                string failurMsg = string.Empty;
                string sucessMsg = string.Empty;
                StringBuilder sb = new StringBuilder();
                string successMsg = string.Empty;
                foreach (ResponseMessage resposeMsg in response.responseMessages)
                {
                    GenericMessage sentMessage = message[resposeMsg.correlationId];
                    if (sentMessage != null)
                    {
                        if (resposeMsg.statusMessage.Contains("Failure"))
                        {
                            jBossMachineName = resposeMsg.machineName;
                            failurMsg = "Records Not Saved" + System.Environment.NewLine;
                            sb.Append(resposeMsg.statusMessage).Append(Environment.NewLine);

                            if (resposeMsg.HasStaleObjects())
                            {
                                failurMsg += buildStaleObjectsErrorMessage(resposeMsg.staleObjectsRecordKey);
                                failurMsg += "Would you like to refresh the objects and Save?";
                                MessageBoxResult result = MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.YesNo);
                                if (result == MessageBoxResult.Yes)
                                {
                                    foreach (string recordKey in resposeMsg.staleObjectsRecordKey)
                                    {
                                        EntityRecord localGenericRecord = dataStoreService.GetGenericObject(recordKey) as EntityRecord;
                                        if (localGenericRecord != null)
                                        {
                                            ServiceUtil.FetchStaleObject<EntityRecord>(localGenericRecord);
                                            int? transId = null;

                                            try
                                            {
                                                transId = Convert.ToInt32(localGenericRecord.GetType().GetProperty("transId").GetValue(localGenericRecord, null));

                                            }
                                            catch (Exception e)
                                            {
                                            }

                                            string logmsg = "StaleObject -> " + recordKey + " New TransId : " + transId;
                                        }
                                    }
                                    DeleteLibraryFormulaData(formualCtl);
                                    return;
                                }
                            }
                            else
                            {
                                if (resposeMsg.validationResults != null)
                                {
                                    failurMsg = string.Empty;
                                    //looping through all the validation results and adding to the display message
                                    foreach (tc.bean.validation.ValidationResult vr in response.responseMessages[0].validationResults.results)
                                    {
                                        failurMsg += vr.ToString() + System.Environment.NewLine;
                                    }

                                    MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.OK);
                                    isFormualDeleted = false;
                                }
                            }
                        }//end of failure block
                        else if (resposeMsg.statusMessage.Contains("Success"))
                        {
                            if (resposeMsg.validationResults.results != null)
                            {
                                foreach (tc.bean.validation.ValidationResult vr in resposeMsg.validationResults.results)
                                {
                                    successMsg += vr.ToString() + System.Environment.NewLine;
                                }
                                string msg = "Formula# " + formula.formulaNum + " deleted successfully";
                                //MessageBox.Show(msg, "Trade Capture", MessageBoxButton.OK);                               
                                ValidationMessageHolder.Instance.ClearAndAddNewValidationMessage("Formula deleted successfully", msg, Severity.Sucess);
                                isFormualDeleted = true;
                                //if (TradeCaptureUtil.LibraryFormulas.Contains(formula))
                                //  TradeCaptureUtil.LibraryFormulas.Remove(formula);
                                foreach (Formula f in TradeCaptureUtil.LibraryFormulas)
                                {
                                    if (f.RecordKey.Equals(formula.RecordKey))
                                    {
                                        TradeCaptureUtil.LibraryFormulas.Remove(f);
                                        break;
                                    }
                                }

                            }
                        }
                    }
                }
            }
            if (isFormualDeleted)
            {
                //Window window = formualCtl.Parent as Window;
                //if (window != null)
                //    window.Close();
                if (formualCtl is ILibraryFormulaSaveCtl)
                {
                    if (((ILibraryFormulaSaveCtl)formualCtl).getFormulaEditor() != null)
                    {
                        Window editorWindow = Window.GetWindow(((ILibraryFormulaSaveCtl)formualCtl).getFormulaEditor());

                        Window parentWindow = Window.GetWindow(formualCtl);
                        if (parentWindow != null)
                        {
                            parentWindow.Close();
                        }
                        if (editorWindow != null)
                        {
                            editorWindow.Close();
                        }
                    }
                }
            }
        }
        private void ProcessDeleteTrade(Trade trade, IDataStoreService dataService)
        {
            bool isTradeDeleted = false;
            //IDataStoreService dataService = container.Resolve<IDataStoreService>();
            ValidationMessageHolder.Instance.RemoveFailureAndWarningValidationMessage();
            if (trade.isInDb == false)
            {
                try
                {
                    dataService.RemoveObjectAndRelations(trade);
                    MessageBox.Show("Trade deleted");
                    isTradeDeleted = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Trade deleted : " + ex.StackTrace.ToString());
                    isTradeDeleted = false;
                }
            }
            else
            {
                List<GenericMessage> listOfMessages = new List<GenericMessage>();
                DeleteTrade deleteTrade = new DeleteTrade();
                deleteTrade.TradeNum = trade.tradeNum.Value;
                deleteTrade.TradeRecordKey = trade.RecordKey;
                listOfMessages.Add(deleteTrade);

                CompositeMessage message = new CompositeMessage();
                message.Messages = listOfMessages;
                IMessageProcessorService processService = container.Resolve<IMessageProcessorService>();
                CompositeResponseMessage response = null;
                response = processService.ProcessDeleteTrade(message, dataService);

                string jBossMachineName = null;
                string failurMsg = string.Empty;
                string sucessMsg = string.Empty;
                StringBuilder sb = new StringBuilder();
                string successMsg = string.Empty;
                foreach (ResponseMessage resposeMsg in response.responseMessages)
                {
                    GenericMessage sentMessage = message[resposeMsg.correlationId];
                    if (sentMessage != null)
                    {
                        if (resposeMsg.statusMessage.Contains("Failure"))
                        {
                            jBossMachineName = resposeMsg.machineName;
                            failurMsg = "Records Not Saved" + System.Environment.NewLine;
                            sb.Append(resposeMsg.statusMessage).Append(Environment.NewLine);

                            if (resposeMsg.HasStaleObjects())
                            {
                                failurMsg += buildStaleObjectsErrorMessage(resposeMsg.staleObjectsRecordKey);
                                failurMsg += "Would you like to refresh the objects and Save?";
                                MessageBoxResult result = MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.YesNo);
                                if (result == MessageBoxResult.Yes)
                                {
                                    foreach (string recordKey in resposeMsg.staleObjectsRecordKey)
                                    {
                                        EntityRecord localGenericRecord = dataService.GetGenericObject(recordKey) as EntityRecord;
                                        if (localGenericRecord != null)
                                        {
                                            ServiceUtil.FetchStaleObject<EntityRecord>(localGenericRecord);
                                            int? transId = null;

                                            try
                                            {
                                                transId = Convert.ToInt32(localGenericRecord.GetType().GetProperty("transId").GetValue(localGenericRecord, null));

                                            }
                                            catch (Exception e)
                                            {
                                            }

                                            string logmsg = "StaleObject -> " + recordKey + " New TransId : " + transId;
                                        }
                                    }
                                    ProcessDeleteTrade(trade, dataService);
                                    return;
                                }
                            }
                            else
                            {
                                if (resposeMsg.validationResults != null)
                                {
                                    failurMsg = string.Empty;
                                    //looping through all the validation results and adding to the display message
                                    foreach (tc.bean.validation.ValidationResult vr in response.responseMessages[0].validationResults.results)
                                    {
                                        //failurMsg += vr.ToString() + System.Environment.NewLine;
                                        ValidationMessageHolder.Instance.AddValidationMessage(vr.validationName, vr.validationMessage, Severity.Failure);
                                    }

                                    //MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.OK);
                                    isTradeDeleted = false;
                                }
                            }
                        }//end of failure block
                        else if (resposeMsg.statusMessage.Contains("Success"))
                        {
                            if (resposeMsg.validationResults.results != null)
                            {
                                foreach (tc.bean.validation.ValidationResult vr in resposeMsg.validationResults.results)
                                {
                                    successMsg += vr.ToString() + System.Environment.NewLine;
                                }
                                string msg = null;
                                if (!trade.inhouseInd.Equals("I"))
                                    msg = "Trade #" + trade.tradeNum + " deleted successfully";
                                else
                                    msg = "Trade #" + trade.tradeNum + "/" + trade.oppositeTrade.tradeNum + " deleted successfully";
                                //MessageBox.Show(msg, "Trade Capture", MessageBoxButton.OK);
                                isTradeDeleted = true;
                            }
                        }
                    }
                }
            }
            if (isTradeDeleted == true)
            {

                if (_panelService != null && _panelService.DockSites != null && _panelService.DockSites.Count > 0)
                {
                    if (trade != null && trade.tradeNum != null)
                    {
                        BackgroundWorker _worker = new BackgroundWorker();
                        _worker.WorkerReportsProgress = false;
                        _worker.WorkerSupportsCancellation = true;
                        _worker.DoWork += new DoWorkEventHandler(BackgroundInit_DoWorkToStoreUserResources);

                        if (!trade.inhouseInd.Equals("I"))
                            _worker.RunWorkerAsync(new object[] { trade.tradeNum + " - Deleted" });
                        else if (trade.tradeNum > trade.oppositeTrade.tradeNum)
                            _worker.RunWorkerAsync(new object[] { trade.oppositeTrade.tradeNum + " / " + trade.tradeNum + " - Deleted" });
                        else
                            _worker.RunWorkerAsync(new object[] { trade.tradeNum + " / " + trade.oppositeTrade.tradeNum + " - Deleted" });
                    }

                    DockWindow dw = (_panelService.DockSites[0] as DockSite).ActiveDockWindow;
                    if (dw != null)
                    {
                        if (dw.Header.ToString().EndsWith("*"))
                        {
                            dw.Header = dw.Header.ToString().Remove(dw.Header.ToString().Length - 1);
                        }
                        dw.Close();
                    }

                    if (dw == null && trade.inhouseInd.Equals("I"))
                    {
                        foreach (DockWindow dwindow in (_panelService.DockSites[0] as DockSite).GetDockWindows())
                        {

                            if ((dwindow.Header.ToString().Contains("Trade#: " + trade.tradeNum) && dwindow.Header.ToString().EndsWith("*")) && dwindow.IsSelected)
                            {
                                dw.Header = dw.Header.ToString().Remove(dw.Header.ToString().Length - 1);
                                dwindow.Close();
                                break;
                            }
                            else if ((dwindow.Header.ToString().Contains("Trade#: " + trade.oppositeTrade.tradeNum) && dwindow.Header.ToString().EndsWith("*")) && dwindow.IsSelected)
                            {
                                dw.Header = dw.Header.ToString().Remove(dw.Header.ToString().Length - 1);
                                dwindow.Close();
                                break;
                            }

                        }
                    }

                }
            }

        }

        public void OpenTradePanelByTradeNumber(string[] tradeNumbers)
        {
            int oid = 0;
            if (tradeNumbers != null && tradeNumbers.Length > 0)
            {
                foreach (String oidString in tradeNumbers)
                {
                    if (oidString != null && !oidString.Trim().Equals(string.Empty))
                    {
                        try
                        {
                            oid = Convert.ToInt32(oidString);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Invalid Trade Number: " + oidString, null);
                            return;
                        }
                        if (oid > 0)
                        {
                            try
                            {
                                string recordkey = string.Format("Trade:tradeNum={0};", oid);
                                Trade trade = null;
                                bool mustRefetch = false;
                                string mergeType = GenericRecord.UserChangesMergeOverwriteUserChanges;
                                //Check if the trade objects is already present in the datastore, if not then fetch it from cache else fetch it from db
                                IMybatisDataAccessService MybatisDataAccessService = container.Resolve<IMybatisDataAccessService>();
                                //IDataStoreService dataStore = container.Resolve<IDataStoreService>();
                                //if (dataStore != null)
                                //    trade = (Trade)dataStore.GetGenericObject("Trade", recordkey);

                                //if (trade == null || trade.IsDirty)
                                //{
                                //    mustRefetch = true;
                                //    mergeType = GenericRecord.UserChangesMergeOverwriteUserChanges;
                                //}
                                //else
                                //{
                                //    mustRefetch = false;
                                //}
                                List<IFormPanel> formPanels = _panelService.GetAllOpenPanels("IFormPanel");
                                foreach (IFormPanel ifp in formPanels)
                                {
                                    if (ifp.FormView is TradeFormView)
                                    {
                                        if (ifp.ViewName != null && (ifp.ViewName.Equals("Trade#: " + oid) || ifp.ViewName.Equals("* Trade#: " + oid) || ifp.ViewName.Equals("Trade#: " + oid + "*")))
                                        {
                                            foreach (DockWindow dwindow in (_panelService.DockSites[0] as DockSite).GetDockWindows())
                                            {
                                                if (dwindow.Header.ToString().Equals(ifp.ViewName))
                                                {
                                                    dwindow.IsSelected = true;
                                                    break;
                                                }
                                            }
                                            return;
                                        }
                                    }
                                }
                                IEntityTagService entityTagService = EntityTagHelper.EntityTagService;
                                CSFetchSpecification fetchSpecification = new CSFetchSpecification("Trade", "Tradelayout", SpecificationTypes.System);
                                fetchSpecification.Qualifier = new CSKeyValueQualifier("t.trade_num", CSQualifierOperatorSelectors.QualifierOperatorEqual, oid);
                                List<Trade> tradeRecords = MybatisDataAccessService.FetchForQuery<Trade>(fetchSpecification, true, true, cs.fw.eo.EOUtil.GetResultMapName("Trade"), false, mergeType, true);
                                if (tradeRecords != null && tradeRecords.Count > 0)
                                {
                                    trade = tradeRecords[0];
                                    if (PortfolioUtil.tradingEntityList == null)
                                        PortfolioUtil.getTradingEntityListBasedOnIctsUserPermission();
                                    if (!PortfolioUtil.isSuperUser)
                                    {
                                        bool canOpen = canOpenTradeBasedOnTradingEntityPermissions(trade);
                                        if (!canOpen)
                                        {
                                            MessageBox.Show("Trade cannot be opened,Please check the permissions.");
                                            return;
                                        }
                                    }
                                    //fetching all VOUCHED, PAID  costs on this trade                                    
                                    CSAndQualifier andQualifier = new CSAndQualifier();
                                    CSOrQualifier orQualifier = new CSOrQualifier();
                                    andQualifier.Qualifiers.Add(new CSKeyValueQualifier("cost_owner_key6", CSQualifierOperatorSelectors.QualifierOperatorEqual, trade.tradeNum));
                                    orQualifier.Qualifiers.Add(new CSKeyValueQualifier("cost_status", CSQualifierOperatorSelectors.QualifierOperatorEqual, "VOUCHED"));
                                    orQualifier.Qualifiers.Add(new CSKeyValueQualifier("cost_status", CSQualifierOperatorSelectors.QualifierOperatorEqual, "PAID"));
                                    andQualifier.Qualifiers.Add(orQualifier);
                                    int? noOfCosts = (int?)ServiceUtil.GetSqlFetchRecord(andQualifier, "CostsCount");
                                    if (noOfCosts != null && noOfCosts > 0)
                                        trade.ContainsVouchedCost = true;

                                    //trade.getDetailDataViaIBatis(container);
                                    //GetTradeOrderChildData(trade);
                                    //if (trade.inhouseInd.Equals("I"))
                                    //{
                                    //    fetchSpecification.Qualifier = new CSKeyValueQualifier("t.trade_num", CSQualifierOperatorSelectors.QualifierOperatorEqual, trade.oppositeTradeNum);
                                    //    List<Trade> tradeRecordsList = MybatisDataAccessService.FetchForQuery<Trade>(fetchSpecification, true, true, cs.fw.eo.EOUtil.GetResultMapName("Trade"), false, mergeType, true);
                                    //    //trade.oppositeTrade = (tradeRecordsList != null && tradeRecordsList.Count > 0) ? tradeRecordsList[0] : null;  
                                    //    if (tradeRecordsList != null && tradeRecordsList.Count > 0)
                                    //    {
                                    //        trade.oppositeTrade = tradeRecordsList[0];
                                    //        trade.oppositeTrade.SetPropertyForBasicTypes(trade.traderInit, "oppositeUserInit");//[ADSO-773]
                                    //        trade.oppositeTrade.getDetailDataViaIBatis(container);
                                    //        GetTradeOrderChildData(trade.oppositeTrade);
                                    //    }
                                    //}
                                }
                                else
                                {
                                    MessageBox.Show("Trade #" + oid + " does not exist in the DataBase.", "Trade Capture", MessageBoxButton.OK);
                                    return;
                                }
                                if (trade != null && trade.tradeStatusCode != null && trade.tradeStatusCode.Trim().Equals("DELETE"))
                                {
                                    MessageBox.Show("Cannot open Trade #" + trade.tradeNum + "- Trade #" + trade.tradeNum + " is deleted.", "Trade Capture", MessageBoxButton.OK);
                                    return;
                                }
                                string isFromTradeFormPanel = string.Empty;
                                //if (trade == null)
                                //{
                                //    MessageBox.Show("Trade # " + oid + " not found!", null, MessageBoxButton.OK, MessageBoxImage.Information);
                                //    return;
                                //}
                                Application.Current.Dispatcher.BeginInvoke(new ThreadStart(() => OpenTradeFormView(trade)), null);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Trade # " + oidString + " couldn't be opened because of the following reason.\n" + ex.Message, null, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show(tradeStatusMessage, null, MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }
                }
            }
        }

        public static bool canOpenTradeBasedOnTradingEntityPermissions(Trade trade)
        {
            bool canOpen = true;
            List<TradeItem> tradeItems = trade.TradeItems;
            if (tradeItems != null && tradeItems.Count > 0)
            {
                foreach (TradeItem tItem in tradeItems)
                {
                    if (tItem.realPortNum != null)
                    {
                        RealPortfolio _realPortfolio = DataStoreServiceHelper.DataStoreService.GetGenericObject("RealPortfolio", "RealPortfolio:portNum=" + tItem.realPortNum.ToString() + ";") as RealPortfolio;
                        if (_realPortfolio != null && _realPortfolio.portLocked != null && _realPortfolio.portLocked.Equals(0))
                        {
                            if (_realPortfolio.tradingEntityNum != null && !PortfolioUtil.tradingEntityList.Contains(_realPortfolio.tradingEntityNum.ToString()))
                            {
                                canOpen = false;
                                return canOpen;
                            }
                        }

                    }
                }
            }
            return canOpen;
        }


        public void NewAsCopyTradePanelByTradeNumber(string[] tradeNumbers)
        {
            NewAsCopyTradePanelByTradeNumber(tradeNumbers, false);
        }

        public void NewAsCopyStdFldsTradePanelByTradeNumber(string[] tradeNumbers)
        {
            NewAsCopyTradePanelByTradeNumber(tradeNumbers, true);
        }

        public void NewAsCopyTradePanelByTradeNumber(string[] tradeNumbers, bool isStdFldsCopy)
        {
            int oid = 0;
            if (tradeNumbers != null && tradeNumbers.Length > 0)
            {
                foreach (String oidString in tradeNumbers)
                {
                    if (oidString != null && !oidString.Trim().Equals(string.Empty))
                    {
                        try
                        {
                            oid = Convert.ToInt32(oidString);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Invalid Trade Number: " + oidString, null);
                        }
                        if (oid > 0 && checkTradeExists(oid))
                        {
                            try
                            {
                                string recordkey = string.Format("Trade:tradeNum={0};", oid);
                                Trade trade = null;
                                string failurMsg = string.Empty;
                                string successMsg = string.Empty;
                                string jBossMachineName = string.Empty;
                                StringBuilder sb = new StringBuilder();
                                List<GenericMessage> listOfMessages = new List<GenericMessage>();
                                CreateNewTrade newTrade = new CreateNewTrade();
                                newTrade.TradeNum = oid;
                                newTrade.IsStdFldsCopy = isStdFldsCopy;
                                //newTrade.TradeRecordKey = recordkey;
                                listOfMessages.Add(newTrade);

                                CompositeMessage message = new CompositeMessage();
                                message.Messages = listOfMessages;
                                IMessageProcessorService processService = container.Resolve<IMessageProcessorService>();
                                CompositeResponseMessage response = null;
                                response = processService.ProcessNewAsCopyTrade(message);

                                foreach (ResponseMessage resposeMsg in response.responseMessages)
                                {
                                    //GenericMessage sentMessage = message[resposeMsg.correlationId];
                                    //if (sentMessage != null)
                                    //{
                                    if (resposeMsg.statusMessage.Contains("Failure"))
                                    {
                                        jBossMachineName = resposeMsg.machineName;
                                        failurMsg = "Trade not Found" + System.Environment.NewLine;
                                        sb.Append(resposeMsg.statusMessage).Append(Environment.NewLine);
                                        if (resposeMsg.validationResults != null)
                                        {
                                            failurMsg = string.Empty;
                                            //looping through all the validation results and adding to the display message
                                            foreach (tc.bean.validation.ValidationResult vr in response.responseMessages[0].validationResults.results)
                                            {
                                                failurMsg += vr.ToString() + System.Environment.NewLine;
                                            }

                                            MessageBox.Show(failurMsg, "Trade Capture", MessageBoxButton.OK);
                                        }

                                    }//end of failure block
                                    else if (resposeMsg.statusMessage.Contains("Success"))
                                    {
                                        if (resposeMsg.validationResults.results != null)
                                        {
                                            foreach (tc.bean.validation.ValidationResult vr in resposeMsg.validationResults.results)
                                            {
                                                successMsg += vr.ToString() + System.Environment.NewLine;
                                            }
                                            if (resposeMsg.affectedObjects != null && resposeMsg.affectedObjects.Count > 0)
                                            {
                                                trade = resposeMsg.affectedObjects[0] as Trade;
                                                TradeDefaultController defaultCtl = TradeDefaultController.newInstance();
                                                defaultCtl.defaultCreditTermsInTrade(trade);

                                                if (trade != null)
                                                {
                                                    //single line added for issue 1399926
                                                    trade.tradeStatusCode = "UNALLOC";

                                                    if (trade.inhouseInd.Equals("I"))
                                                    {
                                                        if (trade.internalChildTrades != null && trade.internalChildTrades.Count > 0)
                                                            trade.oppositeTrade = trade.internalChildTrades[0];
                                                        if (trade.internalParentTrade != null)
                                                            trade.oppositeTrade = trade.internalParentTrade;
                                                    }

                                                    //code modified for issue 1396727.
                                                    //not setting the orderNum and itemNum here instead doing it in java deep copy
                                                    if (trade.tradeOrders != null)
                                                    {
                                                        int? maxOrderNum = 0;
                                                        foreach (TradeOrder aTradeOrder in trade.tradeOrders)
                                                        {
                                                            if (aTradeOrder != null)
                                                            {
                                                                int? maxItemNum = 0;
                                                                if (aTradeOrder.orderNum > maxOrderNum)
                                                                {
                                                                    maxOrderNum = aTradeOrder.orderNum;
                                                                }

                                                                foreach (TradeItem aTradeItem in aTradeOrder.tradeItems)
                                                                {

                                                                    if (aTradeItem != null)
                                                                    {
                                                                        if (aTradeItem.itemNum > maxItemNum)
                                                                        {
                                                                            maxItemNum = aTradeItem.itemNum;
                                                                        }
                                                                        //changes by Raju K on 10Jul14
                                                                        //I#1397338 - Tradingprd is null due to RiskTradeingPrd is null. Here filling RiskTradingPeriod again to avoid right trading prd.
                                                                        //[I#ADSO-555] -- Commented the below condition and 
                                                                        //implemented the logic inside setRiskTradingPeriod() , which is similar as OC
                                                                        //if (aTradeItem.riskTradingPeriod == null && aTradeItem.tradingPrd != null)
                                                                        aTradeItem.setRiskTradingPeriod();
                                                                        aTradeItem.createdAsCopyOf = true;

                                                                        foreach (Cost acost in aTradeItem.costs)
                                                                        {
                                                                            if (acost.costAmtType != null && acost.costAmtType.Equals("f") && acost.accumulation != null)
                                                                            {
                                                                                acost.ResetAccumulation();
                                                                            }
                                                                        }


                                                                        foreach (TradeFormula aTradeFormula in aTradeItem.tradeFormulas)
                                                                        {
                                                                            if (aTradeFormula != null && aTradeFormula.formula != null)
                                                                            {
                                                                                List<Formula> subFormulaList = new List<Formula>();
                                                                                foreach (FormulaBody aFormulaBody in aTradeFormula.formula.formulaBodies)
                                                                                {
                                                                                    if (aFormulaBody != null && aFormulaBody.formulaComponents != null)
                                                                                    {
                                                                                        foreach (FormulaComponent aFormulaComponent in aFormulaBody.formulaComponents)
                                                                                        {
                                                                                            if (aFormulaComponent != null && aFormulaComponent.subFormula != null)
                                                                                            {
                                                                                                if (aFormulaComponent.formulaCompName.Equals("SwapSellFloat"))
                                                                                                {
                                                                                                    ((CashPhysicalTradeItem)aTradeItem).sellFormula = aFormulaComponent.subFormula;
                                                                                                    subFormulaList.Add(aFormulaComponent.subFormula);
                                                                                                }
                                                                                                else if (aFormulaComponent.formulaCompName.Equals("SwapBuyFloat"))
                                                                                                {
                                                                                                    ((CashPhysicalTradeItem)aTradeItem).buyFormula = aFormulaComponent.subFormula;
                                                                                                    subFormulaList.Add(aFormulaComponent.subFormula);
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                aTradeFormula.formula.subFormulaArray = subFormulaList;
                                                                            }
                                                                        }
                                                                        aTradeItem.SetParentItem();
                                                                        if (aTradeItem is DryPhysicalTradeItem)
                                                                        {
                                                                            aTradeItem.FetchCommoditySpecs();
                                                                        }
                                                                    }
                                                                }
                                                                aTradeOrder.maxItemNum = maxItemNum;
                                                            }
                                                        }
                                                        trade.maxOrderNum = maxOrderNum;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    // }
                                }
                                //if (trade == null)
                                //{
                                //    MessageBox.Show("Trade # " + oid + " not found!", null, MessageBoxButton.OK, MessageBoxImage.Information);
                                //    return;
                                //}
                                Application.Current.Dispatcher.BeginInvoke(new ThreadStart(() => OpenTradeFormView(trade)), null);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Trade # " + oidString + " couldn't be opened because of the following reason.\n" + ex.Message, null, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show(tradeStatusMessage, null, MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }
                }
            }
        }

        private void GetTradeOrderChildData(Trade trade)
        {
            if (trade.tradeOrders != null && trade.tradeOrders.Count > 0)
            {
                for (int i = 0; i < trade.tradeOrders.Count; i++)
                {
                    trade.tradeOrders[i].getDetailDataViaIBatis(container);
                    if(trade.tradeOrders[i].IsSummary)
                    {
                       foreach(TradeItem ti in trade.tradeOrders[i].tradeItems)
                       {
                           ti.getDetailDataViaIBatis(container);
                       }
                    }
                }
            }
        }
        private void OpenTradeFormView(Trade trade)
        {
            if (System.Windows.Forms.Cursor.Current == System.Windows.Forms.Cursors.Default)
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

            IFormView newTradePanel = new TradeFormView(_panelService, container);
            newTradePanel.container = this.container;
            IWindowService windowSvc = container.Resolve<IWindowService>();
            if (trade != null)
            {
                //if (trade.TradeItems.Count > 0 && trade.TradeItems[0].itemType.Equals("F"))
                //{
                //    if (trade.TradeItems[0].tradeOrder.orderTypeCode.Equals("EFPEXCH"))
                //        EFPFutureTradePanelCtl.formView = newTradePanel;                 
                //    else
                //        FutureTradePanelCtl.formView = newTradePanel;  
                //}
                //else if (trade.TradeItems.Count > 0 && trade.TradeItems[0].itemType.Equals("X"))
                //    EFPFutureTradePanelCtl.formView = newTradePanel;
                //else if (trade.TradeItems.Count > 0 && trade.TradeItems[0].itemType.Equals("E"))
                //    ListedOptionsTradePanelCtl.formView = newTradePanel;
                //else if (trade.TradeItems.Count > 0 && trade.TradeItems[0].itemType.Equals("W"))

                if (trade.inhouseInd.Equals("I"))
                {
                    IMybatisDataAccessService MybatisDataAccessService = container.Resolve<IMybatisDataAccessService>();
                    CSFetchSpecification fetchSpecification = new CSFetchSpecification("Trade", "Tradelayout", SpecificationTypes.System);
                    string mergeType = GenericRecord.UserChangesMergeOverwriteUserChanges;
                    fetchSpecification.Qualifier = new CSKeyValueQualifier("t.trade_num", CSQualifierOperatorSelectors.QualifierOperatorEqual, trade.oppositeTradeNum);
                    List<Trade> tradeRecordsList = MybatisDataAccessService.FetchForQuery<Trade>(fetchSpecification, true, true, cs.fw.eo.EOUtil.GetResultMapName("Trade"), false, mergeType, true);
                    if (tradeRecordsList != null && tradeRecordsList.Count > 0)
                    {
                        trade.oppositeTrade = tradeRecordsList[0];
                        if (trade.oppositeTrade != null)
                            trade.oppositeTrade.oppositeTrade = trade;
                        trade.oppositeTrade.SetPropertyForBasicTypes(trade.traderInit, "oppositeUserInit");//[ADSO-773]
                        //trade.oppositeTrade.getDetailDataViaIBatis(container);
                        //GetTradeOrderChildData(trade.oppositeTrade);
                    }
                }
                if (trade.TradeItems.Count > 0 && trade.TradeItems[0].itemType.Equals("W"))
                {
                    //if (trade.TradeItems[0].tradeOrder.orderTypeCode.Equals("EFPEXCH"))
                    //    EFPTradePanelCtl.formView = newTradePanel;
                    //else
                    //    PhysicalBuySellTradePanelCtl.formView = newTradePanel;

                    foreach (TradeItem ti in trade.TradeItems)
                    {
                        if (ti is WetPhysicalTradeItem)
                        {
                            (ti as WetPhysicalTradeItem).getAutopoolDataViaIBatis(container);
                            break;
                        }

                    }
                }
                //else if (trade.TradeItems.Count > 0 && trade.TradeItems[0].itemType.Equals("O"))
                //OTCOptionCashSettledAmerEuroTradePanelCtl.formView = newTradePanel;
                //else if (trade.TradeItems.Count > 0 && trade.TradeItems[0].itemType.Equals("C") && trade.TradeItems[0].tradeOrder != null)
                //if (trade.TradeItems[0].tradeOrder.orderTypeCode.Equals("SWAP"))
                //SwapFixedFloatTradePanelCtl.formView = newTradePanel;
                //else if (trade.TradeItems[0].tradeOrder.orderTypeCode.Equals("SWAPFLT"))
                // SwapFloatFloatTradePanelCtl.formView = newTradePanel;
                ((tc.m.tradeCapture.TradeFormView)(newTradePanel)).tradePanelCntrl.tradeOrderListCtl.TradeItemInfoPanel = ((tc.m.tradeCapture.TradeFormView)(newTradePanel)).tradePanelCntrl.tradeDetailsGrid;

                Trade latestTrade = null;
                //IDataStoreService dataStore = container.Resolve<IDataStoreService>();
                //BEWARE - doing this will revert any changes you did to the Trade object after we got values from Java
                //use with caution
                //if (dataStore != null)
                //    latestTrade = (Trade)dataStore.GetGenericObject("Trade", trade.RecordKey);

                //latestTrade = (latestTrade != null) ? latestTrade : trade;
                latestTrade = trade;
                if (latestTrade != null)
                {
                    BeginInitOnTrade(latestTrade);
                    if (latestTrade.oppositeTrade != null)
                        BeginInitOnTrade(latestTrade.oppositeTrade);

                    (newTradePanel.dataStoreService as IDataStoreService).MergeGenericObject(latestTrade);
                    if (latestTrade.oppositeTrade != null)
                        (newTradePanel.dataStoreService as IDataStoreService).MergeGenericObject(latestTrade.oppositeTrade);

                    EndInitOnTrade(latestTrade);
                    if (latestTrade.oppositeTrade != null)
                        EndInitOnTrade(latestTrade.oppositeTrade);

                    latestTrade.SelectedTradeItem.setTradeDefaults("commodity");
                    newTradePanel.SetDataSource(latestTrade);
                    latestTrade.dataStoreService = newTradePanel.dataStoreService;
                    //venkat
                    latestTrade.getDetailDataViaIBatis(container);
                    GetTradeOrderChildData(latestTrade);
                    if (latestTrade.inhouseInd.Equals("I") && latestTrade.oppositeTrade != null)
                    {
                        latestTrade.oppositeTrade.dataStoreService = newTradePanel.dataStoreService;
                        //venkat
                        latestTrade.oppositeTrade.getDetailDataViaIBatis(container);
                        GetTradeOrderChildData(latestTrade.oppositeTrade);
                    }
                    if (latestTrade.isInDb)
                        windowSvc.AddPanelToWindow(container, null, (latestTrade as EntityRecord).DisplayParameter, newTradePanel);
                    else
                        windowSvc.AddPanelToWindow(container, null, "New", newTradePanel);
                    object[] param = new object[3];
                    param[0] = latestTrade.tradeNum;
                    param[1] = newTradePanel.ViewName;
                    param[2] = "REGISTER";
                    //LQSFormRegistration(param);
                }
            }
            else
            {
                windowSvc.AddPanelToWindow(container, null, "New", newTradePanel);
            }
        }

        private void BeginInitOnTrade(Trade trade)
        {
            trade.BeginInit();
            if (trade.tradeOrders != null)
            {
                foreach (TradeOrder order in trade.tradeOrders)
                {
                    order.BeginInit();
                    if (order.tradeItems != null)
                    {
                        foreach (TradeItem item in order.tradeItems)
                        {
                            item.BeginInit();
                        }
                    }
                }
            }
        }
        private void EndInitOnTrade(Trade trade)
        {
            trade.EndInit();
            if (trade.tradeOrders != null)
            {
                foreach (TradeOrder order in trade.tradeOrders)
                {
                    order.EndInit();
                    if (order.tradeItems != null)
                    {
                        foreach (TradeItem item in order.tradeItems)
                        {
                            item.EndInit();
                        }
                    }
                }
            }
        }

        public void OpenTradePanel(TradeOrderOptionsSelected orderOption)
        {
            if (orderOption != null && orderOption.Trade == null)
            {
                CreateTradeAndOpenPanel(orderOption);
            }
            else if (orderOption != null && orderOption.Trade != null)
            {
                AddOrderOrItemToTrade(orderOption.treeViewCtl, orderOption);
            }
        }

        private void CreateTradeAndOpenPanel(TradeOrderOptionsSelected orderOption)
        {
            if (orderOption != null)
            {
                Trade trade = null;
                switch (orderOption.OrderType)
                {
                    case "PHYSEXCH":
                        trade = PhysicalTradesBusiness.GetPhysicalExchangeTrade(container, orderOption);
                        break;
                    case "SWAP":
                        trade = SwapTradeBusiness.GetCashPhysicalTrade(container, orderOption);
                        break;
                    case "SWAPFLT":
                        trade = SwapTradeBusiness.GetCashPhysicalTrade(container, orderOption);
                        break;
                    case "FUTURE":
                        trade = FutureTradeBusiness.GetFutureTrade(container);
                        break;
                    case "EXCHGOPT":
                        trade = ExchangeOptionTradeBusiness.GetExchangeOptionTrade(container);
                        break;
                    case "STORAGE":
                        trade = StorageAgreementTradeBusiness.GetStorageAgreementnTrade(container);
                        break;
                    case "PIPELINE":
                        trade = TransportAgreementBusiness.GetTransportTrade(container, "PIPELINE");
                        break;
                    case "BARGE":
                        trade = TransportAgreementBusiness.GetTransportTrade(container, "BARGE");
                        break;
                    case "TRUCK":
                        trade = TransportAgreementBusiness.GetTransportTrade(container, "TRUCK");
                        break;
                    case "PHYSICAL":
                        trade = PhysicalTradesBusiness.GetPhysicalBuySelTrade(container, orderOption);
                        break;
                    case "PHYSCONC":
                        trade = PhysicalTradesBusiness.GetPhysicalBuySelTrade(container, orderOption);
                        break;
                    case "PHYSBUNK":
                        trade = PhysicalTradesBusiness.GetPhysicalBuySelTrade(container, orderOption);
                        break;
                    case "PARTIAL":
                        trade = PhysicalTradesBusiness.GetPhysicalBuySelTrade(container, orderOption);
                        break;
                    case "VLCC":
                        trade = TransportAgreementBusiness.GetTransportTrade(container, "VLCC");
                        break;
                    case "RAILCAR":
                        trade = TransportAgreementBusiness.GetTransportTrade(container, "RAILCAR");
                        break;
                    case "EFPEXCH":
                        trade = PhysicalTradesBusiness.GetPhysicalBuySelTrade(container, orderOption);
                        break;
                    case "OTCCASH":
                        trade = OtcOptionTradeBusiness.GetOtcOptionTrade(container, orderOption);
                        break;
                    case "OTCAPO":
                        trade = OtcOptionTradeBusiness.GetOtcOptionTrade(container, orderOption);
                        break;
                    case "OTCPHYS_PHYSICAL":
                        trade = OtcOptionTradeBusiness.GetOtcOptionPhysicallySettledTrade(container, orderOption, "PHYSICAL");
                        break;
                    case "OTCPHYS_PARTIAL":
                        trade = OtcOptionTradeBusiness.GetOtcOptionPhysicallySettledTrade(container, orderOption, "PARTIAL");
                        break;
                    case "CURRENCY":
                        trade = CurrencyTradeBusiness.GetCurrencyTrade(container);
                        break;
                    case "OTCPHYS_SWAP":
                        trade = OtcOptionTradeBusiness.GetOtcOptionPhysicallySettledSwapTrade(container, orderOption, "SWAP");
                        break;
                    case "OTCPHYS_SWAPFLT":
                        trade = OtcOptionTradeBusiness.GetOtcOptionPhysicallySettledSwapTrade(container, orderOption, "SWAPFLT");
                        break;
                }

                foreach (TradeItem ti in trade.TradeItems)
                {
                    ti.useMktFormulaForPl = "Y";
                }
                Application.Current.Dispatcher.BeginInvoke(new ThreadStart(() => OpenTradeFormView(trade)), null);
            }
        }

        public void OpenAddHedgePhysical(UserControl assocTradeCtl)
        {
            TradeItem tradeItem = assocTradeCtl.DataContext as TradeItem;

            HedgePhysical hedgePhysical = new HedgePhysical();
            decimal? hedgePhysRemainPcnt = (decimal)0;
            decimal? hedgePhysAddedPcnt = (decimal)0;
            if (tradeItem != null)
            {
                switch (tradeItem.itemType)
                {
                    case "C":   //Swaps
                        if ((tradeItem as CashPhysicalTradeItem).hedgePhysicals != null)
                        {
                            foreach (HedgePhysical hedgePhys in (tradeItem as CashPhysicalTradeItem).hedgePhysicals)
                            {
                                hedgePhysAddedPcnt += hedgePhys.weightPcnt;
                            }
                        }
                        break;

                    case "E":   //Listed options
                        if ((tradeItem as ExchangeOptionTradeItem).hedgePhysicals != null)
                        {
                            foreach (HedgePhysical hedgePhys in (tradeItem as ExchangeOptionTradeItem).hedgePhysicals)
                            {
                                hedgePhysAddedPcnt += hedgePhys.weightPcnt;
                            }
                        }
                        break;

                    case "F":   //FUTURE
                        if ((tradeItem as FutureTradeItem).hedgePhysicals != null)
                        {
                            foreach (HedgePhysical hedgePhys in (tradeItem as FutureTradeItem).hedgePhysicals)
                            {
                                hedgePhysAddedPcnt += hedgePhys.weightPcnt;
                            }
                        }
                        break;

                    case "O":   //OTC option
                        if ((tradeItem as OtcOptionTradeItem).hedgePhysicals != null)
                        {
                            foreach (HedgePhysical hedgePhys in (tradeItem as OtcOptionTradeItem).hedgePhysicals)
                            {
                                hedgePhysAddedPcnt += hedgePhys.weightPcnt;
                            }
                        }
                        break;

                    case "U":   //Currency
                        if ((tradeItem as CurrencyTradeItem).hedgePhysicals != null)
                        {
                            foreach (HedgePhysical hedgePhys in (tradeItem as CurrencyTradeItem).hedgePhysicals)
                            {
                                hedgePhysAddedPcnt += hedgePhys.weightPcnt;
                            }
                        }
                        break;
                }

                hedgePhysRemainPcnt = 100 - hedgePhysAddedPcnt;
                if (tradeItem.brkrCommUom != tradeItem.contractQtyUom)
                    tradeItem.brkrCommUom = tradeItem.contractQtyUom;

                if (hedgePhysRemainPcnt > 0)
                {
                    hedgePhysical.weightPcnt = hedgePhysRemainPcnt;
                    hedgePhysical.tradeItem = tradeItem;
                    switch (tradeItem.itemType)
                    {
                        case "F":   //FUTURE
                            if ((tradeItem as FutureTradeItem).hedgePhysicals == null)
                                (tradeItem as FutureTradeItem).hedgePhysicals = new List<HedgePhysical>();
                            (tradeItem as FutureTradeItem).hedgePhysicals.Add(hedgePhysical);
                            (tradeItem as FutureTradeItem).hedgePhysicals = new List<HedgePhysical>((tradeItem as FutureTradeItem).hedgePhysicals);
                            break;
                        case "C":   //Swaps
                            if ((tradeItem as CashPhysicalTradeItem).hedgePhysicals == null)
                                (tradeItem as CashPhysicalTradeItem).hedgePhysicals = new List<HedgePhysical>();
                            (tradeItem as CashPhysicalTradeItem).hedgePhysicals.Add(hedgePhysical);
                            (tradeItem as CashPhysicalTradeItem).hedgePhysicals = new List<HedgePhysical>((tradeItem as CashPhysicalTradeItem).hedgePhysicals);
                            break;

                        case "E":   //Listed options
                            if ((tradeItem as ExchangeOptionTradeItem).hedgePhysicals == null)
                                (tradeItem as ExchangeOptionTradeItem).hedgePhysicals = new List<HedgePhysical>();
                            (tradeItem as ExchangeOptionTradeItem).hedgePhysicals.Add(hedgePhysical);
                            (tradeItem as ExchangeOptionTradeItem).hedgePhysicals = new List<HedgePhysical>((tradeItem as ExchangeOptionTradeItem).hedgePhysicals);
                            break;
                        case "O":   //OTC option
                            if ((tradeItem as OtcOptionTradeItem).hedgePhysicals == null)
                                (tradeItem as OtcOptionTradeItem).hedgePhysicals = new List<HedgePhysical>();
                            (tradeItem as OtcOptionTradeItem).hedgePhysicals.Add(hedgePhysical);
                            (tradeItem as OtcOptionTradeItem).hedgePhysicals = new List<HedgePhysical>((tradeItem as OtcOptionTradeItem).hedgePhysicals);
                            break;
                        case "U":   //Currency
                            if ((tradeItem as CurrencyTradeItem).hedgePhysicals == null)
                                (tradeItem as CurrencyTradeItem).hedgePhysicals = new List<HedgePhysical>();
                            (tradeItem as CurrencyTradeItem).hedgePhysicals.Add(hedgePhysical);
                            (tradeItem as CurrencyTradeItem).hedgePhysicals = new List<HedgePhysical>((tradeItem as CurrencyTradeItem).hedgePhysicals);
                            break;
                    }

                    IDataStoreService datService = (IDataStoreService)tradeItem.trade.dataStoreService;
                    //datService.MergeGenericObject(hedgePhysical);
                    (assocTradeCtl as AssociationTradesCtl).RefreshDataSource();
                }
                else
                    MessageBox.Show("Cannot add more hedge physical, because the total weight is already 100%.", "TradeCapture", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void OpenDeleteHedgePhysical(UserControl assocTradeCtl)
        {
            TradeItem tradeItem = ((assocTradeCtl as AssociationTradesCtl).DataContext as TradeItem);
            HedgePhysical hedgePhys = null;
            if ((assocTradeCtl as AssociationTradesCtl) != null && (assocTradeCtl as AssociationTradesCtl).associationTradesGrid != null && (assocTradeCtl as AssociationTradesCtl).associationTradesGrid.SelectedItem != null)
                hedgePhys = (assocTradeCtl as AssociationTradesCtl).associationTradesGrid.SelectedItem as HedgePhysical;
            if (hedgePhys != null)
            {
                switch (tradeItem.itemType)
                {
                    case "F":   //FUTURE
                        if ((tradeItem as FutureTradeItem).hedgePhysicals != null)
                            (tradeItem as FutureTradeItem).hedgePhysicals.Remove(hedgePhys);
                        (tradeItem as FutureTradeItem).hedgePhysicals = new List<HedgePhysical>((tradeItem as FutureTradeItem).hedgePhysicals);
                        break;
                    case "C":   //Swaps
                        if ((tradeItem as CashPhysicalTradeItem).hedgePhysicals != null)
                            (tradeItem as CashPhysicalTradeItem).hedgePhysicals.Remove(hedgePhys);
                        (tradeItem as CashPhysicalTradeItem).hedgePhysicals = new List<HedgePhysical>((tradeItem as CashPhysicalTradeItem).hedgePhysicals);
                        break;
                    case "E":   //Listed options
                        if ((tradeItem as ExchangeOptionTradeItem).hedgePhysicals != null)
                            (tradeItem as ExchangeOptionTradeItem).hedgePhysicals.Remove(hedgePhys);
                        (tradeItem as ExchangeOptionTradeItem).hedgePhysicals = new List<HedgePhysical>((tradeItem as ExchangeOptionTradeItem).hedgePhysicals);
                        break;
                    case "O":   //OTC option
                        if ((tradeItem as OtcOptionTradeItem).hedgePhysicals != null)
                            (tradeItem as OtcOptionTradeItem).hedgePhysicals.Remove(hedgePhys);
                        (tradeItem as OtcOptionTradeItem).hedgePhysicals = new List<HedgePhysical>((tradeItem as OtcOptionTradeItem).hedgePhysicals);
                        break;
                    case "U":   //Currency
                        if ((tradeItem as CurrencyTradeItem).hedgePhysicals != null)
                            (tradeItem as CurrencyTradeItem).hedgePhysicals.Remove(hedgePhys);
                        (tradeItem as CurrencyTradeItem).hedgePhysicals = new List<HedgePhysical>((tradeItem as CurrencyTradeItem).hedgePhysicals);
                        break;
                }
                IDataStoreService datService = (IDataStoreService)tradeItem.trade.dataStoreService;
                if (hedgePhys.isInDb)
                    datService.AddToDeletedObjectsList(hedgePhys);
                else
                    datService.RemoveObjectAndRelations(hedgePhys);
                (assocTradeCtl as AssociationTradesCtl).RefreshDataSource();
            }

        }
        //private TradingPeriod GetTradingPeriod(TradeItem tradeItem,string riskMktCode)
        //{
        //    TradingPeriodBusiness tradingPrdBusiness = new TradingPeriodBusiness();
        //    TradingPeriod tradingPrd = tradingPrdBusiness.GetTradingPeriod(tradeItem.cmdtyCode, riskMktCode, container);
        //    return tradingPrd;
        //}


        //private void GetAllDirtyObjects(GenericRecord genericRecord, List<GenericRecord> dirtyObjs)
        //{
        //    if (genericRecord.IsDirty == true)
        //        dirtyObjs.Add(genericRecord);
        //    List<PropertyInfo> propItems = GetNonPrimitiveProperties(genericRecord);
        //    if (propItems != null && propItems.Count > 0)
        //    {
        //        foreach (PropertyInfo property in propItems)
        //        {
        //            GenericRecord item = genericRecord.valueForKey<GenericRecord>(property.Name);
        //            List<GenericRecord> items = genericRecord.valueForKey<List<GenericRecord>>(property.Name);
        //            //Object item = property.GetValue(genericRecord, null);
        //            if (item is List<GenericRecord>)
        //            {
        //            }
        //            else
        //            {
        //                GetAllDirtyObjects(item as GenericRecord, dirtyObjs);
        //            }
        //        }
        //    }
        //}

        //private List<PropertyInfo> GetNonPrimitiveProperties(GenericRecord genericRecord)
        //{
        //    List<PropertyInfo> propItems = new List<PropertyInfo>();

        //    Type entity = genericRecord.GetType();
        //    foreach (PropertyInfo property in entity.GetProperties())
        //    {
        //        if (property.PropertyType.IsGenericType)
        //        {
        //            PropertyInfo prop = property.PropertyType.GetProperty("Item");
        //            if (prop != null && !property.Name.Equals("Self"))
        //            {
        //                Type temp = prop.PropertyType;
        //                Object[] attributes = property.GetCustomAttributes(typeof(DisplayHierarchyAttribute), true);
        //                if (temp != null && ((temp == typeof(GenericRecord)) || (temp.IsSubclassOf(typeof(GenericRecord)))))
        //                {
        //                    propItems.Add(property);
        //                }
        //            }
        //        }
        //    }
        //    return propItems;
        //}

        public static void ExchangeDataContextForInternalTrades(object sender, Trade _trade, Trade currentDataContext, IFormView formView)
        {
            if (formView != null)
            {
                if ((sender as RadioButton).Content.Equals("Main"))
                {
                    if (_trade.internalParentTradeNum == null)
                        currentDataContext = _trade;
                    else
                    {
                        if (currentDataContext.oppositeTrade.oppositeTradeNum != null && currentDataContext.oppositeTrade.oppositeTrade == null)
                        {
                            currentDataContext.oppositeTrade.oppositeTrader = _trade.trader;
                        }
                        currentDataContext = currentDataContext.oppositeTrade;
                        int? orderNum = _trade.SelectedTradeItem.orderNum;
                        int? itemNum = _trade.SelectedTradeItem.itemNum;
                        foreach (TradeItem ti in currentDataContext.TradeItems)
                        {
                            if (ti.orderNum == orderNum && ti.itemNum == itemNum)
                            {
                                currentDataContext.SelectedTradeItem = ti;
                                break;
                            }
                        }

                    }
                }
                else
                {
                    //[I#ADSO-83] code modified(temporary fix) by Kiran Kumar Poloju on 12th Nov 2014 
                    // if (_trade.oppositeTrade.internalParentTradeNum != null)
                    //reverted the above issue fix and implemented logic at server side
                    if (_trade != null && _trade.internalParentTradeNum != null)
                    {
                        _trade.oppositeTrader = _trade.oppositeTrade.trader;
                        currentDataContext = _trade;
                    }
                    else
                    {
                        //[ADSO-773]: bellow code commented to avoid making opposite trade dirty and moved logic to trade loading time
                        //if (currentDataContext.oppositeTrade != null)
                        //{
                        //    currentDataContext.oppositeTrade.trader = _trade.oppositeTrader;  
                        //    currentDataContext.oppositeTrade.oppositeTrader = _trade.trader;                                             

                        //}                   
                        currentDataContext = currentDataContext.oppositeTrade;
                        //Issue#ADSO-1161 - CS2.4:Regression: Buy and Sell indicator is not getting changed when turning to opposite 
                        //mode for future leg of EFP internal trade.
                        int? orderNum = _trade.SelectedTradeItem.orderNum;
                        int? itemNum = _trade.SelectedTradeItem.itemNum;
                        foreach (TradeItem ti in currentDataContext.TradeItems)
                        {
                            if (ti.orderNum == orderNum && ti.itemNum == itemNum)
                            {
                                currentDataContext.SelectedTradeItem = ti;
                                break;
                            }
                        }

                        //currentDataContext.internalParentTrade = _trade;
                    }

                }

                ((tc.m.tradeCapture.TradeFormView)(formView)).DataContext = currentDataContext;
                IPanelService _panelService = ((tc.m.tradeCapture.TradeFormView)(formView)).PanelService;
                if (_panelService != null && _panelService.DockSites != null && _panelService.DockSites.Count > 0)
                {
                    DockWindow dw = (_panelService.DockSites[0] as DockSite).ActiveDockWindow;
                    if (dw != null &&  !dw.Header.ToString().Contains("Trade Search"))
                    {
                        if (currentDataContext.IsDirty)
                            dw.Header = String.Format("* Trade#: {0}", currentDataContext.tradeNum);
                        else
                            dw.Header = String.Format("Trade#: {0}", currentDataContext.tradeNum);
                    }
                }
            }
        }

        public void OpenAddSpecs(UserControl tradeItemSpecsCtl)
        {
            if (((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext as TradeItem).cmdtyCode == null)
            {
                MessageBox.Show("Please enter a commodity first!", "TradeCapture");
                return;
            }
            if ((tradeItemSpecsCtl as TradeItemSpecsCtl).specsGrid != null)
            {
                if ((tradeItemSpecsCtl as TradeItemSpecsCtl).specsGrid.ItemsSource == null)
                    (tradeItemSpecsCtl as TradeItemSpecsCtl).specsGrid.ItemsSource = new List<TradeItemSpec>();
            }
            TradeItemSpec tiSpec = new TradeItemSpec();
            if ((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext is WetPhysicalTradeItem)
            {
                tiSpec.tradeItem = ((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext as WetPhysicalTradeItem);
                //((physicalTradesSpecsCtl as PhysicalTradesSpecsCtl).DataContext as WetPhysicalTradeItem).specifications.Add(tiSpec);              
                if (((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext as WetPhysicalTradeItem).specifications == null)
                    ((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext as WetPhysicalTradeItem).specifications = new List<TradeItemSpec>();
                ((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext as WetPhysicalTradeItem).addToSpecifications(tiSpec);
            }
            else if ((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext is DryPhysicalTradeItem)
            {
                tiSpec.tradeItem = ((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext as DryPhysicalTradeItem);
                //((physicalTradesSpecsCtl as PhysicalTradesSpecsCtl).DataContext as WetPhysicalTradeItem).specifications.Add(tiSpec);              
                if (((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext as DryPhysicalTradeItem).specifications == null)
                    ((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext as DryPhysicalTradeItem).specifications = new List<TradeItemSpec>();
                ((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext as DryPhysicalTradeItem).addToSpecifications(tiSpec);
            }
            else if ((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext is StorageTradeItem)
            {
                tiSpec.tradeItem = ((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext as StorageTradeItem);
                //((physicalTradesSpecsCtl as PhysicalTradesSpecsCtl).DataContext as WetPhysicalTradeItem).specifications.Add(tiSpec);              
                if (((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext as StorageTradeItem).specifications == null)
                    ((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext as StorageTradeItem).specifications = new List<TradeItemSpec>();
                ((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext as StorageTradeItem).addToSpecifications(tiSpec);
            }
            else if ((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext is TransportTradeItem)
            {
                tiSpec.tradeItem = ((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext as TransportTradeItem);
                //((physicalTradesSpecsCtl as PhysicalTradesSpecsCtl).DataContext as WetPhysicalTradeItem).specifications.Add(tiSpec);              
                if (((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext as TransportTradeItem).specifications == null)
                    ((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext as TransportTradeItem).specifications = new List<TradeItemSpec>();
                ((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext as TransportTradeItem).addToSpecifications(tiSpec);
            }


            (tradeItemSpecsCtl as TradeItemSpecsCtl).RefreshDataSource();
            TradeItemSpecsCtl tiSpecCtl = tradeItemSpecsCtl as TradeItemSpecsCtl;
            tiSpecCtl.specsGrid.SelectedItem = tiSpec;
            tiSpecCtl.specsGrid.Focus();
            foreach (GridColumn col in tiSpecCtl.specsGridview.VisibleColumns)
            {
                if (col.ReadOnly == false)
                {
                    tiSpecCtl.specsGridview.FocusedColumn = col;
                    break;
                }
            }
            tiSpecCtl.specsGridview.ShowEditor();

        }
        public void OpenDeleteSpecs(UserControl tradeItemSpecsCtl)
        {
            TradeItem tradeItem = tradeItemSpecsCtl.DataContext as TradeItem;

            if (tradeItem != null && (tradeItemSpecsCtl as TradeItemSpecsCtl).specsGrid != null)
            {
                TradeItemSpec tiSpec = null;
                TradeItemSpec nxtSpectoSelect = null;
                List<TradeItemSpec> allItems = null;
                if ((tradeItemSpecsCtl as TradeItemSpecsCtl).specsGrid.SelectedItem != null)
                    tiSpec = (tradeItemSpecsCtl as TradeItemSpecsCtl).specsGrid.SelectedItem as TradeItemSpec;
                if (tiSpec != null)
                {
                    bool isFound = false;
                    if ((tradeItemSpecsCtl as TradeItemSpecsCtl).specsGrid.ItemsSource != null)
                    {
                        allItems = (tradeItemSpecsCtl as TradeItemSpecsCtl).specsGrid.ItemsSource as List<TradeItemSpec>;
                        foreach (TradeItemSpec tSpec in allItems)
                        {
                            if (isFound)
                            {
                                nxtSpectoSelect = tSpec;
                                break;
                            }
                            if (tSpec != null && tSpec.Equals(tiSpec))
                            {
                                isFound = true;
                            }
                        }
                    }
                    IDataStoreService datService = (IDataStoreService)tradeItem.trade.dataStoreService;
                    if ((tradeItem is WetPhysicalTradeItem) && (tradeItem as WetPhysicalTradeItem).specifications != null)
                    {
                        (tradeItem as WetPhysicalTradeItem).removeFromSpecifications(tiSpec);
                        RemoveFromDataStore(datService, tiSpec);
                    }
                    else if ((tradeItem is DryPhysicalTradeItem) && (tradeItem as DryPhysicalTradeItem).specifications != null)
                    {
                        (tradeItem as DryPhysicalTradeItem).removeFromSpecifications(tiSpec);
                        RemoveFromDataStore(datService, tiSpec);
                    }
                    else if ((tradeItem is StorageTradeItem) && (tradeItem as StorageTradeItem).specifications != null)
                    {
                        (tradeItem as StorageTradeItem).removeFromSpecifications(tiSpec);
                        RemoveFromDataStore(datService, tiSpec);
                    }
                    else if ((tradeItem is TransportTradeItem) && (tradeItem as TransportTradeItem).specifications != null)
                    {
                        (tradeItem as TransportTradeItem).removeFromSpecifications(tiSpec);
                        RemoveFromDataStore(datService, tiSpec);
                    }
                    //[I#1399693] code changes for displaying the warning message for strip trades after modification. code changes by Kiran Kumar Poloju on 12-Aug-2014
                    if (tradeItem.tradeOrder != null && tradeItem.tradeOrder.IsSummary && !(tradeItem.tradeOrder.IsApplyChanges))
                    {
                        tradeItem.tradeOrder.IsApplyChanges = true;
                        tradeItem.tradeOrder.NotifyPropertyChanged("IsApplyChanges");
                    }
                    //[I#1399693] code end
                    (tradeItemSpecsCtl as TradeItemSpecsCtl).RefreshDataSource();
                    if (nxtSpectoSelect != null)
                        (tradeItemSpecsCtl as TradeItemSpecsCtl).specsGrid.SelectedItem = nxtSpectoSelect;
                    else if ((tradeItemSpecsCtl as TradeItemSpecsCtl).specsGrid.VisibleRowCount > 0)
                        (tradeItemSpecsCtl as TradeItemSpecsCtl).specsGrid.SelectedItem = allItems[(tradeItemSpecsCtl as TradeItemSpecsCtl).specsGrid.VisibleRowCount - 1];
                }
            }
        }

        //Modfied below code for the issue : ADSO-145, also changed the code as per the Oc behaviour
        // Modified By devender M

        public void OpenAddAllSpecs(UserControl tradeItemSpecsCtl)
        {
            if ((tradeItemSpecsCtl as TradeItemSpecsCtl).specsGrid != null)
            {
                if ((tradeItemSpecsCtl as TradeItemSpecsCtl).specsGrid.ItemsSource == null)
                    (tradeItemSpecsCtl as TradeItemSpecsCtl).specsGrid.ItemsSource = new List<TradeItemSpec>();
            }
            if ((tradeItemSpecsCtl as TradeItemSpecsCtl).DataContext is TradeItem)
            {
                List<TradeItemSpec> _existingSpecs = new List<TradeItemSpec>();
                TradeItem tradeItem = tradeItemSpecsCtl.DataContext as TradeItem;
                ObservableCollection<CommoditySpecification> tiCommoditySpecs = null;
                if (tradeItem.cmdtyCode != null)
                {
                    WetPhysicalTradeItem tiWetPhy = null;
                    StorageTradeItem tiStorage = null;
                    TransportTradeItem tiTransport = null;
                    List<TradeItemSpec> _specs = null;

                    if (tradeItem.GetType().Name.Equals("WetPhysicalTradeItem"))
                    {
                        tiWetPhy = (tradeItem as WetPhysicalTradeItem);
                        _specs = tiWetPhy.specifications;
                        tiCommoditySpecs = tiWetPhy.CommoditySpecificationsList;
                        tiWetPhy.specifications = null;
                    }
                    if (tradeItem is DryPhysicalTradeItem)
                    {
                        _specs = (tradeItem as DryPhysicalTradeItem).specifications;
                        tiCommoditySpecs = (tradeItem as DryPhysicalTradeItem).CommoditySpecificationsList;
                        //(tradeItem as DryPhysicalTradeItem).specifications = null;
                    }
                    else if (tradeItem.GetType().Name.Equals("StorageTradeItem"))
                    {
                        tiStorage = (tradeItem as StorageTradeItem);
                        _specs = tiStorage.specifications;
                        tiCommoditySpecs = tiStorage.CommoditySpecificationsList;
                        tiStorage.specifications = null;
                    }
                    else if (tradeItem.GetType().Name.Equals("TransportTradeItem"))
                    {
                        tiTransport = (tradeItem as TransportTradeItem);
                        _specs = tiTransport.specifications;
                        tiCommoditySpecs = tiTransport.CommoditySpecificationsList;
                        tiTransport.specifications = null;
                    }
                    if (_specs != null)
                    {
                        foreach (TradeItemSpec aSpec in _specs)
                        {
                            if (aSpec != null && aSpec.specification != null)
                                _existingSpecs.Add(aSpec);
                        }
                    }

                    if (tiCommoditySpecs != null && tiCommoditySpecs.Count > 0)
                    {


                        foreach (CommoditySpecification cmdtySpec in tiCommoditySpecs)
                        {

                            if (_existingSpecs != null && _existingSpecs.Count > 0)
                            {
                                TradeItemSpec tradeItemSpec = _existingSpecs.Where(t => t.specification != null && t.specification.specCode == cmdtySpec.specCode).FirstOrDefault();
                                if (tradeItemSpec != null)
                                {
                                    if (tiWetPhy != null)
                                    {
                                        if (tiWetPhy.specifications == null)
                                            tiWetPhy.specifications = new List<TradeItemSpec>();
                                        tiWetPhy.specifications.Add(tradeItemSpec);
                                    }
                                    if (tiStorage != null)
                                    {
                                        if (tiStorage.specifications == null)
                                            tiStorage.specifications = new List<TradeItemSpec>();
                                        tiStorage.specifications.Add(tradeItemSpec);
                                    }
                                    if (tiTransport != null)
                                    {
                                        if (tiTransport.specifications == null)
                                            tiTransport.specifications = new List<TradeItemSpec>();
                                        tiTransport.specifications.Add(tradeItemSpec);
                                    }
                                }
                                else
                                    AddTiSpecs(tradeItem, cmdtySpec);
                            }
                            else
                                AddTiSpecs(tradeItem, cmdtySpec);

                        }
                    }
                    else
                        MessageBox.Show("The commodity {0} of the this trade item does not have any specifications to copy in." + tradeItem.cmdtyCode, "TradeCapture");
                }
                else
                {
                    MessageBox.Show("Please enter a commodity first!", "TradeCapture");
                    return;
                }

                //(tradeItem as WetPhysicalTradeItem).SpecificationsForCommodityList = PhysicalTradeUtil.getCommoditySpecifications(Container, tradeItem.cmdtyCode);
                //(tradeItem as WetPhysicalTradeItem).SpecificationsList = tradeItem.SpecificationsForCommodityList;
                /*if ((tradeItem is WetPhysicalTradeItem) && (tradeItem as WetPhysicalTradeItem).CommoditySpecificationsList != null && (tradeItem as WetPhysicalTradeItem).CommoditySpecificationsList.Count > 0)
                {

                    foreach (CommoditySpecification cmdtySpec in (tradeItem as WetPhysicalTradeItem).CommoditySpecificationsList)
                    {
                        List<TradeItemSpec> tiSpecs = ((tradeItem as WetPhysicalTradeItem).specifications != null) ? (tradeItem as WetPhysicalTradeItem).specifications.Where(tis => tis.specification != null && tis.specification.Equals(cmdtySpec.specification)).ToList() : null;
                        if (tiSpecs == null || tiSpecs.Count == 0)
                        {
                            if (_existingSpecs != null && _existingSpecs.Count > 0)
                            {
                                TradeItemSpec tradeItemSpec = _existingSpecs.Where(t => t.specification != null && t.specification.specCode == cmdtySpec.specCode).FirstOrDefault();
                                if (tradeItemSpec != null)
                                {
                                    if ((tradeItem as WetPhysicalTradeItem).specifications == null)
                                        (tradeItem as WetPhysicalTradeItem).specifications = new List<TradeItemSpec>();
                                    (tradeItem as WetPhysicalTradeItem).specifications.Add(tradeItemSpec);
                                }
                                else
                                    AddTiSpecs(tradeItem, cmdtySpec);
                            }
                            else
                            AddTiSpecs(tradeItem, cmdtySpec);
                        }
                    }
                }
                else if ((tradeItem is StorageTradeItem) && (tradeItem as StorageTradeItem).CommoditySpecificationsList != null && (tradeItem as StorageTradeItem).CommoditySpecificationsList.Count > 0)
                {
                    if ((tradeItem as StorageTradeItem).specifications != null)
                    {
                        _existingSpecs = new List<TradeItemSpec>();
                        foreach (TradeItemSpec item in (tradeItem as StorageTradeItem).specifications)
                        {
                            if (item != null && item.specification != null)
                                _existingSpecs.Add(item);

                        }
                    }
                    (tradeItem as StorageTradeItem).specifications = null;
                    foreach (CommoditySpecification cmdtySpec in (tradeItem as StorageTradeItem).CommoditySpecificationsList)
                    {
                        List<TradeItemSpec> tiSpecs = ((tradeItem as StorageTradeItem).specifications != null) ? (tradeItem as StorageTradeItem).specifications.Where(tis => tis.specification != null && tis.specification.Equals(cmdtySpec.specification)).ToList() : null;
                        if (tiSpecs == null || tiSpecs.Count == 0)
                        {
                            if (_existingSpecs != null && _existingSpecs.Count > 0)
                            {
                                TradeItemSpec tradeItemSpec = _existingSpecs.Where(t => t.specification != null && t.specification.specCode == cmdtySpec.specCode).FirstOrDefault();
                                if (tradeItemSpec != null)
                                {
                                    if ((tradeItem as StorageTradeItem).specifications == null)
                                        (tradeItem as StorageTradeItem).specifications = new List<TradeItemSpec>();
                                    (tradeItem as StorageTradeItem).specifications.Add(tradeItemSpec);
                                }
                                else
                                    AddTiSpecs(tradeItem, cmdtySpec);
                            }
                            else
                                AddTiSpecs(tradeItem, cmdtySpec);
                        }
                    }
                }
                else if ((tradeItem is TransportTradeItem) && (tradeItem as TransportTradeItem).CommoditySpecificationsList != null && (tradeItem as TransportTradeItem).CommoditySpecificationsList.Count > 0)
                {
                    if ((tradeItem as TransportTradeItem).specifications != null)
                    {
                        _existingSpecs = new List<TradeItemSpec>();
                        foreach (TradeItemSpec item in (tradeItem as TransportTradeItem).specifications)
                        {
                            if (item != null && item.specification != null)
                                _existingSpecs.Add(item);

                        }
                    }
                    (tradeItem as TransportTradeItem).specifications = null;
                    foreach (CommoditySpecification cmdtySpec in (tradeItem as TransportTradeItem).CommoditySpecificationsList)
                    {
                        List<TradeItemSpec> tiSpecs = ((tradeItem as TransportTradeItem).specifications != null) ? (tradeItem as TransportTradeItem).specifications.Where(tis => tis.specification != null && tis.specification.Equals(cmdtySpec.specification)).ToList() : null;
                        if (tiSpecs == null || tiSpecs.Count == 0)
                        {
                            if (_existingSpecs != null && _existingSpecs.Count > 0)
                            {
                                TradeItemSpec tradeItemSpec = _existingSpecs.Where(t => t.specification != null && t.specification.specCode == cmdtySpec.specCode).FirstOrDefault();
                                if (tradeItemSpec != null)
                                {
                                    if ((tradeItem as TransportTradeItem).specifications == null)
                                        (tradeItem as TransportTradeItem).specifications = new List<TradeItemSpec>();
                                    (tradeItem as TransportTradeItem).specifications.Add(tradeItemSpec);
                                }
                                else
                                    AddTiSpecs(tradeItem, cmdtySpec);
                            }
                            else
                                AddTiSpecs(tradeItem, cmdtySpec);
                        }
                    }
                }

                else
                    MessageBox.Show("The commodity {0} of the this trade item does not have any specifications to copy in." + tradeItem.cmdtyCode, "TradeCapture");
            }
                else
                {
                    MessageBox.Show("Please enter a commodity first!", "TradeCapture");
                    return;
                }*/

                (tradeItemSpecsCtl as TradeItemSpecsCtl).RefreshDataSource();
            }
        }

        private void AddTiSpecs(TradeItem tradeItem, CommoditySpecification cmdtySpec)
        {
            TradeItemSpec tiSpec = null;
            if (tradeItem.isInDb == true)
            {
                string recKey = "TradeItemSpec:tradeNum=" + tradeItem.tradeNum + ";orderNum=" + tradeItem.orderNum + ";itemNum=" + tradeItem.itemNum + ";specCode="
                            + cmdtySpec.specCode + ";";
                TradeItemSpec removedSpec = tradeItem.dataStoreService.GetDeletedObject(recKey) as TradeItemSpec;
                if (removedSpec != null)
                {
                    (tradeItem.dataStoreService as IDataStoreService).RemoveObjectFromDeletedObjects(removedSpec);
                    tiSpec = removedSpec;
                }
                else
                    tiSpec = new TradeItemSpec();
            }
            else
                tiSpec = new TradeItemSpec();


            tiSpec.tradeItem = tradeItem;
            tiSpec.specCode = cmdtySpec.specCode;
            tiSpec.specification = cmdtySpec.specification;
            tiSpec.specMaxVal = cmdtySpec.cmdtySpecMaxVal;
            tiSpec.specMinVal = cmdtySpec.cmdtySpecMinVal;
            if (cmdtySpec.cmdtySpecMinVal != null && cmdtySpec.cmdtySpecMaxVal != null && cmdtySpec.cmdtySpecTypicalVal >= cmdtySpec.cmdtySpecMinVal && cmdtySpec.cmdtySpecTypicalVal <= cmdtySpec.cmdtySpecMaxVal)
                tiSpec.specTypicalVal = cmdtySpec.cmdtySpecTypicalVal;
            else if (cmdtySpec.cmdtySpecTypicalVal != null)
                tiSpec.specTypicalVal = cmdtySpec.cmdtySpecTypicalVal;
            else
                tiSpec.specTypicalVal = null;
            if (cmdtySpec.dfltSpecTestCode != null)
                tiSpec.specTestCode = cmdtySpec.dfltSpecTestCode;
            if (tradeItem is WetPhysicalTradeItem)
            {
                if ((tradeItem as WetPhysicalTradeItem).specifications == null)
                    (tradeItem as WetPhysicalTradeItem).specifications = new List<TradeItemSpec>();
                (tradeItem as WetPhysicalTradeItem).addToSpecifications(tiSpec);
            }
            else if (tradeItem is DryPhysicalTradeItem)
            {
                if ((tradeItem as DryPhysicalTradeItem).specifications == null)
                    (tradeItem as DryPhysicalTradeItem).specifications = new List<TradeItemSpec>();
                (tradeItem as DryPhysicalTradeItem).addToSpecifications(tiSpec);
            }
            else if (tradeItem is StorageTradeItem)
            {
                if ((tradeItem as StorageTradeItem).specifications == null)
                    (tradeItem as StorageTradeItem).specifications = new List<TradeItemSpec>();
                (tradeItem as StorageTradeItem).addToSpecifications(tiSpec);
            }
            else if (tradeItem is TransportTradeItem)
            {
                if ((tradeItem as TransportTradeItem).specifications == null)
                    (tradeItem as TransportTradeItem).specifications = new List<TradeItemSpec>();
                (tradeItem as TransportTradeItem).addToSpecifications(tiSpec);
            }
        }
        public void OpenDeleteAllSpecs(UserControl tradeItemSpecsCtl)
        {
            TradeItem tradeItem = tradeItemSpecsCtl.DataContext as TradeItem;
            if (tradeItem != null)
            {
                IDataStoreService datService = (IDataStoreService)tradeItem.trade.dataStoreService;
                if ((tradeItem is WetPhysicalTradeItem) && (tradeItem as WetPhysicalTradeItem).specifications != null)
                {
                    List<TradeItemSpec> tempLst = (tradeItem as WetPhysicalTradeItem).specifications;
                    foreach (TradeItemSpec tiSpec in tempLst)
                    {
                        RemoveFromDataStore(datService, tiSpec);
                    }
                    tempLst.Clear();
                    (tradeItem as WetPhysicalTradeItem).specifications = new List<TradeItemSpec>(tempLst);
                }
                else if ((tradeItem is DryPhysicalTradeItem) && (tradeItem as DryPhysicalTradeItem).specifications != null)
                {
                    List<TradeItemSpec> tempLst = (tradeItem as DryPhysicalTradeItem).specifications;
                    foreach (TradeItemSpec tiSpec in tempLst)
                    {
                        RemoveFromDataStore(datService, tiSpec);
                    }
                    tempLst.Clear();
                    (tradeItem as DryPhysicalTradeItem).specifications = new List<TradeItemSpec>(tempLst);
                }
                else if (tradeItem != null && (tradeItem is StorageTradeItem) && (tradeItem as StorageTradeItem).specifications != null)
                {
                    List<TradeItemSpec> tempLst = (tradeItem as StorageTradeItem).specifications;
                    foreach (TradeItemSpec tiSpec in tempLst)
                    {
                        RemoveFromDataStore(datService, tiSpec);
                    }
                    tempLst.Clear();
                    (tradeItem as StorageTradeItem).specifications = new List<TradeItemSpec>(tempLst);
                }
                else if (tradeItem != null && (tradeItem is TransportTradeItem) && (tradeItem as TransportTradeItem).specifications != null)
                {
                    List<TradeItemSpec> tempLst = (tradeItem as TransportTradeItem).specifications;
                    foreach (TradeItemSpec tiSpec in tempLst)
                    {
                        RemoveFromDataStore(datService, tiSpec);
                    }
                    tempLst.Clear();
                    (tradeItem as TransportTradeItem).specifications = new List<TradeItemSpec>(tempLst);
                }
                (tradeItemSpecsCtl as TradeItemSpecsCtl).RefreshDataSource();
            }
        }

        private void RemoveFromDataStore(IDataStoreService datService, TradeItemSpec tiSpec)
        {
            if (tiSpec.isInDb)
                datService.AddToDeletedObjectsList(tiSpec);
            else
                datService.RemoveObjectAndRelations(tiSpec);
        }





        /**
        * TODO: describe displayTotalQtyForAllOrderTypes
        */
        public void DisplayTotalQtyForAllOrderTypes(Trade trade)
        {
            StringBuilder aString = new StringBuilder();

            Dictionary<string, object> totalQtyDict = new Dictionary<string, object>();
            //Dictionary<object, object> subDict = null;
            //Dictionary<string, object> typeDict = null;
            //Dictionary<string, object> cmdtyDict = null;

            string aType = null;
            string buySell = null;
            decimal? aQty = 0;
            Uom aUom = null;
            Commodity aCmdty = null;
            try
            {
                if (trade != null && trade.tradeOrders != null && trade.tradeOrders.Count > 0)
                {
                    foreach (TradeOrder to in trade.tradeOrders)
                    {

                        if (to != null && to.tradeItems != null && to.tradeItems.Count > 0)
                        {
                            //aType = to.orderTypeCode;

                            if (!to.IsSummary)
                            {
                                foreach (TradeItem ti in to.tradeItems)
                                {
                                    if (!(ti is EFPFutureTradeItem))
                                    {
                                        if (ti.isParentOTCOptUnExercised() || ti.parentItem != null || (to.orderTypeCode != null && to.orderTypeCode.Equals("STORAGE") && ti.parentItemNum != null))
                                            continue;

                                        aType = TradeCaptureUtil.orderTypeName(to.orderTypeCode);
                                        Dictionary<object, object> subDict = null;
                                        Dictionary<string, object> typeDict = null;
                                        Dictionary<string, object> cmdtyDict = null;
                                        if (to.orderTypeCode != null && to.orderTypeCode.Equals("EFPEXCH"))//EFPEXCHG_OTCODE
                                        {
                                            if (ti is WetPhysicalTradeItem)
                                                aType = aType + "//" + "PHYSICAL";
                                            else
                                                aType = aType + "//" + "FUTURES";
                                        }
                                        if (totalQtyDict.ContainsKey(aType))
                                            typeDict = (Dictionary<string, object>)totalQtyDict[aType];
                                        if (typeDict == null)
                                        {
                                            typeDict = new Dictionary<string, object>();
                                            totalQtyDict.Add(aType, typeDict);
                                        }
                                        //if (!totalQtyDict.ContainsKey(aType))
                                        //    totalQtyDict.Add(aType, typeDict);
                                        if (ti is CashPhysicalTradeItem)
                                        {
                                            List<FormulaBody> buyQuotes = null;
                                            List<FormulaBody> sellQuotes = null;
                                            decimal? factor = null;
                                            if (ti.contrQtyPeriodicity != null && ti.contrQtyPeriodicity.Equals("L") && ti.mainformula != null)
                                            {
                                                AvgBuySellPriceTerm buySellPriceTerm = ti.mainFormula().avgBuySellPriceTermProp;
                                                if (buySellPriceTerm != null && buySellPriceTerm.priceTermStartDate != null
                                                    && buySellPriceTerm.priceTermEndDate != null)
                                                {
                                                    factor = QuantityExtension.extendedAmountForPeriodicityFromToUnitAmount(ti.contrQtyPeriodicity, buySellPriceTerm.priceTermStartDate.Value, buySellPriceTerm.priceTermEndDate.Value, (decimal)1);
                                                }
                                            }
                                            if (ti.isFloatVersusFloat())
                                            {
                                                buyQuotes = ti.buyFloatPriceFormula().quoteBodies();
                                                sellQuotes = ti.sellFloatPriceFormula().quoteBodies();
                                            }
                                            else
                                            {
                                                if (ti.pSInd.Equals("P"))
                                                    sellQuotes = ti.mainFormula().quoteBodies();
                                                else
                                                    buyQuotes = ti.mainFormula().quoteBodies();
                                            }

                                            if (buyQuotes != null && buyQuotes.Count > 0)
                                            {
                                                buySell = "BuyFloat";
                                                if (typeDict.ContainsKey(buySell))
                                                    subDict = (Dictionary<object, object>)typeDict[buySell];
                                                if (subDict == null)
                                                {
                                                    subDict = new Dictionary<object, object>();
                                                    typeDict.Add(buySell, subDict);
                                                }
                                                foreach (FormulaBody fb in buyQuotes)
                                                {
                                                    FormulaComponent aComp = null;
                                                    if (fb.formulaComponents != null && fb.formulaComponents.Count > 0)
                                                        aComp = fb.formulaComponents[fb.formulaComponents.Count - 1];
                                                    if (aComp.commodityMarket != null && aComp.commodityMarket.commodity != null)
                                                        aCmdty = aComp.commodityMarket.commodity;
                                                    aQty = fb.formulaQtyPcntVal;
                                                    aUom = fb.formulaQtyUom;

                                                    if (aUom != null && aUom.uomCode != null && aUom.uomCode.Equals("%"))
                                                    {
                                                        aQty = Decimal.Multiply((decimal)aQty, (decimal)ti.contrQty);
                                                        aUom = ti.contractQtyUom;
                                                    }
                                                    if (factor != null && aQty != null)
                                                        aQty = Decimal.Multiply((decimal)aQty, (decimal)factor);

                                                    if (aQty != null && aUom != null)
                                                    {
                                                        addToSubToalDictionaryQtyUomCommodityTradeItem(subDict, aQty, aUom, aCmdty, ti);
                                                    }
                                                }
                                            }

                                            if (sellQuotes != null && sellQuotes.Count > 0)
                                            {
                                                buySell = "SellFloat";
                                                if (typeDict.ContainsKey(buySell))
                                                    subDict = (Dictionary<object, object>)typeDict[buySell];
                                                if (subDict == null)
                                                {
                                                    subDict = new Dictionary<object, object>();
                                                    typeDict.Add(buySell, subDict);
                                                }
                                                foreach (FormulaBody fb in sellQuotes)
                                                {
                                                    FormulaComponent aComp = null;
                                                    if (fb.formulaComponents != null && fb.formulaComponents.Count > 0)
                                                        aComp = fb.formulaComponents[fb.formulaComponents.Count - 1];
                                                    if (aComp.commodityMarket != null && aComp.commodityMarket.commodity != null)
                                                        aCmdty = aComp.commodityMarket.commodity;
                                                    aQty = fb.formulaQtyPcntVal;
                                                    aUom = fb.formulaQtyUom;

                                                    if (aUom != null && aUom.uomCode != null && aUom.uomCode.Equals("%"))
                                                    {
                                                        aQty = Decimal.Multiply((decimal)aQty, (decimal)ti.contrQty);
                                                        aUom = ti.contractQtyUom;
                                                    }
                                                    if (factor != null && aQty != null)
                                                        aQty = Decimal.Multiply((decimal)aQty, (decimal)factor);

                                                    if (aQty != null && aUom != null)
                                                    {
                                                        addToSubToalDictionaryQtyUomCommodityTradeItem(subDict, aQty, aUom, aCmdty, ti);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            aQty = ti.ContrQtyAmountPerLife();
                                            if (aQty == null)
                                                aQty = ti.contrQty;
                                            aUom = ti.contractQtyUom;
                                            aCmdty = ti.commodity;
                                            if (ti.pSInd.Equals("P"))
                                                buySell = "Buy";
                                            else
                                                buySell = "Sell";

                                            if (ti is OtcOptionTradeItem)
                                            {
                                                TradeItemOtcOpt opt = null;
                                                opt = (TradeItemOtcOpt)ti.tradeItemExtension();
                                                if (opt != null && opt.putCallInd != null && opt.putCallInd.Equals("P"))
                                                    buySell = "Put";
                                                else
                                                    buySell = "Call";
                                            }
                                            else if (ti is ExchangeOptionTradeItem)
                                            {
                                                TradeItemExchOpt opt = null;
                                                opt = (TradeItemExchOpt)ti.tradeItemExtension();
                                                if (opt != null && opt.putCallInd != null && opt.putCallInd.Equals("P"))
                                                    buySell = "Put";
                                                else
                                                    buySell = "Call";
                                            }
                                            if (typeDict.ContainsKey(buySell))
                                                subDict = (Dictionary<object, object>)typeDict[buySell];

                                            if (subDict == null)
                                            {
                                                subDict = new Dictionary<object, object>();
                                                typeDict.Add(buySell, subDict);
                                            }
                                            if (aUom != null)
                                            {
                                                addToSubToalDictionaryQtyUomCommodityTradeItem(subDict, aQty, aUom, aCmdty, ti);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception localException)
            {
                return;
            }

            if (totalQtyDict != null && totalQtyDict.Count > 0)
            {
                Dictionary<object, object> subDict = null;
                Dictionary<string, object> typeDict = null;
                Dictionary<string, object> cmdtyDict = null;
                List<string> allKeys = totalQtyDict.Keys.ToList<string>();
                allKeys.Reverse();
                List<string> e1 = null;
                List<object> e2 = null;
                foreach (string key in allKeys)
                {
                    if (key != null)
                        aType = key;
                    aString.Append("\n    " + aType);
                    if (totalQtyDict.ContainsKey(aType))
                        typeDict = (Dictionary<string, object>)totalQtyDict[aType];
                    if (typeDict != null && typeDict.Count > 0)
                        e1 = typeDict.Keys.ToList<string>();
                    foreach (string buySell1 in e1)
                    {
                        if (typeDict.ContainsKey(buySell1))
                            subDict = (Dictionary<object, object>)typeDict[buySell1];
                        if (subDict != null && subDict.Count > 0)
                        {
                            aString.Append("\n      " + buySell1);
                            int count = 0;
                            e2 = subDict.Keys.ToList<object>();
                            decimal? totalSummedQty = 0;
                            foreach (object cmdtyKey in e2)
                            {
                                if (cmdtyKey != null && cmdtyKey is Commodity)
                                {
                                    count++;
                                    if (subDict.ContainsKey(cmdtyKey))
                                        cmdtyDict = (Dictionary<string, object>)subDict[cmdtyKey];

                                    aString.Append("\n\t" + (cmdtyKey as Commodity).cmdtyShortName + ": ");
                                    if (cmdtyDict.ContainsKey("qty"))
                                    {
                                        if (cmdtyDict["qty"] != null)
                                        {
                                            aString.Append(" " + (decimal)cmdtyDict["qty"]);
                                            totalSummedQty += (decimal)cmdtyDict["qty"];
                                        }
                                    }
                                    if (cmdtyDict.ContainsKey("uom"))
                                        aString.Append(" " + ((Uom)cmdtyDict["uom"]).uomCode);

                                }
                            }
                            if (count != 1 && subDict["qty"] != null)
                            {
                                aString.Append("\n            --------------------------");
                                aString.Append("\n\tSub Total:");
                                if (totalSummedQty != 0)
                                {
                                    if (subDict.ContainsKey("qty"))
                                        subDict["qty"] = totalSummedQty;
                                }
                                if (subDict.ContainsKey("qty"))
                                {
                                    if (subDict["qty"] != null)
                                        aString.Append(" " + (decimal)subDict["qty"]);
                                }
                                if (subDict.ContainsKey("uom"))
                                    aString.Append(" " + ((Uom)subDict["uom"]).uomCode);
                            }
                            aString.Append("\n");
                        }
                    }
                }

            }
            else
                aString.Append("\n 0.00");

            MessageBox.Show("Total Qty: \n " + aString.ToString(), "Trade Capture", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        public void addToSubToalDictionaryQtyUomCommodityTradeItem(Dictionary<object, object> subDict, decimal? aQty, Uom aUom, Commodity aCmdty, TradeItem item)
        {
            Dictionary<string, object> cmdtyDict = null;

            decimal? convertedQty = null;
            decimal? subQty = null;
            Uom subUom = null;
            if (subDict.ContainsKey("uom"))
                subUom = (Uom)subDict["uom"];

            if (subUom == null)
            {
                subUom = aUom;
                if (!subDict.ContainsKey("Uom"))
                    subDict.Add("uom", subUom);
            }
            if (subDict.ContainsKey("qty"))
                subQty = (decimal)subDict["qty"];

            if (subQty == null)
            {
                subQty = aQty;
            }
            else
            {
                if (aUom.Equals(subUom))
                {
                    convertedQty = aQty;
                }
                else
                {
                    decimal factor = item.convertFromUomFromMarketToUomToMarketForCommodity(aUom, item.riskMarket, subUom, item.riskMarket, aCmdty);
                    if (CLOP.test(factor))
                    {
                        if (aQty != null)
                            convertedQty = Decimal.Multiply((decimal)aQty, factor);
                    }
                    else
                    {
                        //  throw new JCException(NSGenericException, String.format("Cannot convert from %s to %s for %s.", aUom, subUom, aCmdty.cmdtyCode()));
                    }
                }
                subQty += convertedQty;
            }

            if (!subDict.ContainsKey("qty"))
                subDict.Add("qty", subQty);

            if (aCmdty != null)
            {
                if (subDict.ContainsKey(aCmdty))
                    cmdtyDict = (Dictionary<string, object>)subDict[aCmdty];

                if (cmdtyDict == null)
                {
                    cmdtyDict = new Dictionary<string, object>();
                    if (!subDict.ContainsKey(aCmdty))
                        subDict.Add(aCmdty, cmdtyDict);
                }
                if (cmdtyDict.ContainsKey("uom"))
                    subUom = (Uom)cmdtyDict["uom"];

                if (subUom == null)
                {
                    subUom = aUom;
                }
                subQty = null;
                if (!cmdtyDict.ContainsKey("uom"))
                    cmdtyDict.Add("uom", subUom);
                if (cmdtyDict.ContainsKey("qty"))
                    subQty = (decimal?)cmdtyDict["qty"];

                if (subQty == null)
                {
                    subQty = aQty;
                }
                else
                {
                    if (aUom.Equals(subUom))
                    {
                        convertedQty = aQty;
                    }
                    else
                    {
                        decimal factor = item.convertFromUomFromMarketToUomToMarketForCommodity(aUom, item.riskMarket, subUom, item.riskMarket, aCmdty);

                        if (CLOP.test(factor))
                        {
                            if (aQty != null)
                                convertedQty = Decimal.Multiply((decimal)aQty, factor);
                        }
                        else
                        {
                            //throw new JCException(NSGenericException, String.format("Cannot convert from %s to %s for %s.", aUom, subUom, aCmdty.cmdtyCode()));
                        }
                    }
                    subQty += convertedQty;
                }
                if (cmdtyDict.ContainsKey("qty"))
                    cmdtyDict["qty"] = subQty;
                else
                    cmdtyDict.Add("qty", subQty);
            }
        }

        //ADSO-88 logic to check the trade is existed or not before going to fetch trade details from server side
        public bool checkTradeExists(int tradeNum)
        {
            bool exists = false;
            tradeStatusMessage = null;
            if (tradeNum != 0)
            {
                string mergeType = GenericRecord.UserChangesMergeOverwriteUserChanges;
                IMybatisDataAccessService MybatisDataAccessService = container.Resolve<IMybatisDataAccessService>();
                CSFetchSpecification fetchSpecification = new CSFetchSpecification("Trade", "Tradelayout", SpecificationTypes.System);
                fetchSpecification.Qualifier = new CSKeyValueQualifier("t.trade_num", CSQualifierOperatorSelectors.QualifierOperatorEqual, tradeNum);
                List<Trade> tradeRecords = MybatisDataAccessService.FetchForQuery<Trade>(fetchSpecification, true, true, cs.fw.eo.EOUtil.GetResultMapName("Trade"), false, mergeType, true);
                if (tradeRecords != null && tradeRecords.Count > 0)
                {
                    string tradeStatusCode = tradeRecords[0].tradeStatusCode;
                    if (tradeStatusCode.Equals("DELETE"))
                    {
                        tradeStatusMessage = "Trade # " + tradeNum + " is deleted";
                    }
                    else
                    {
                        exists = true;
                    }
                }
                else
                {
                    tradeStatusMessage = "Cannot open Trade # " + tradeNum + " - No such trade number " + tradeNum;
                }
            }
            return exists;
        }

        #region IModule Members

        public void Initialize()
        {
            ModuleService.modules.Add(this);
            DragDropServiceHelper.UnityContainer = this.container;
            FutureTradeCommandsHandler tradeCommandsHandler = container.Resolve<FutureTradeCommandsHandler>();
            tradeCommandsHandler.PopupRegionManager = this.regionManager;
            tradeCommandsHandler.Container = this.container;
            tradeCommandsHandler.RefDataService = this.refDataService;


            ListedOptionsTradeCommandsHandler listedOptionsCommandsHandler = container.Resolve<ListedOptionsTradeCommandsHandler>();
            listedOptionsCommandsHandler.PopupRegionManager = this.regionManager;
            listedOptionsCommandsHandler.Container = this.container;
            listedOptionsCommandsHandler.RefDataService = this.refDataService;

            StorageAgreementTradeCommandsHandler strgAggrementCommandsdHandler = container.Resolve<StorageAgreementTradeCommandsHandler>();
            strgAggrementCommandsdHandler.PopupRegionManager = this.regionManager;
            strgAggrementCommandsdHandler.Container = this.container;
            strgAggrementCommandsdHandler.RefDataService = this.refDataService;

            TransportAgreementTradeCommandHandler transportAggrementCommandsdHandler = container.Resolve<TransportAgreementTradeCommandHandler>();
            transportAggrementCommandsdHandler.PopupRegionManager = this.regionManager;
            transportAggrementCommandsdHandler.Container = this.container;
            transportAggrementCommandsdHandler.RefDataService = this.refDataService;

            PhysicalTradeCommandsHandler physicalTradeCommandsHandler = container.Resolve<PhysicalTradeCommandsHandler>();
            physicalTradeCommandsHandler.PopupRegionManager = this.regionManager;
            physicalTradeCommandsHandler.Container = this.container;
            physicalTradeCommandsHandler.RefDataService = this.refDataService;

            FormulaCommandsHandler formulaCommandsHandler = container.Resolve<FormulaCommandsHandler>();
            formulaCommandsHandler.PopupRegionManager = this.regionManager;
            formulaCommandsHandler.Container = this.container;
            formulaCommandsHandler.RefDataService = this.refDataService;

            SwapTradeCommandsHandler swapTradeCommandsHandler = container.Resolve<SwapTradeCommandsHandler>();
            swapTradeCommandsHandler.PopupRegionManager = this.regionManager;
            swapTradeCommandsHandler.Container = this.container;
            swapTradeCommandsHandler.RefDataService = this.refDataService;

            OTCOptionCashSettledTradeCommandsHandler otcOptionCashSettledTradeCommandsHandler = container.Resolve<OTCOptionCashSettledTradeCommandsHandler>();
            otcOptionCashSettledTradeCommandsHandler.PopupRegionManager = this.regionManager;
            otcOptionCashSettledTradeCommandsHandler.Container = this.container;
            otcOptionCashSettledTradeCommandsHandler.RefDataService = this.refDataService;

            PreferencesCommandsHandler preferencesCommandsHandler = container.Resolve<PreferencesCommandsHandler>();
            preferencesCommandsHandler.PopupRegionManager = this.regionManager;
            preferencesCommandsHandler.Container = this.container;
            preferencesCommandsHandler.RefDataService = this.refDataService;

            cs.m.common.physicaltrades.PhysicalTradeCommandsHandler commonPhysicalTradeCommandsHandler = new cs.m.common.physicaltrades.PhysicalTradeCommandsHandler(new cs.m.common.physicaltrades.PhysicalTradeCommandProxy());
            commonPhysicalTradeCommandsHandler.PopupRegionManager = this.regionManager;
            commonPhysicalTradeCommandsHandler.Container = this.container;
            commonPhysicalTradeCommandsHandler.RefDataService = this.refDataService;

            AllTradeTypesCommandsHandler allTradeTypesCommandsHandler = new AllTradeTypesCommandsHandler(new cs.m.common.allTradeTypes.AllTradeTypesCommandProxy());
            allTradeTypesCommandsHandler.PopupRegionManager = this.regionManager;
            allTradeTypesCommandsHandler.Container = this.container;
            allTradeTypesCommandsHandler.RefDataService = this.refDataService;

            RegisterFetchSpecifications();
        }

        public object OpenPanel(Type typeObject, string name, string fetchSpecification, string windowName, Object workspacePModel)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
