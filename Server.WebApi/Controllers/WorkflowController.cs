using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

using Siemplify.Common;
using Siemplify.Common.IoC;
using Siemplify.Server.Common;
using Siemplify.DataModel.Workflows;
using Siemplify.Interfaces;
using Siemplify.Server.WebApi.Infrastructure;
using Siemplify.DataModel.Managment;
using Siemplify.Server.Common.Services;

namespace Siemplify.Server.WebApi.Controllers
{
    [RoutePrefix("api/Workflows")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [SiemplifyAuthorize]
    public class WorkflowController : SiemplifyController, IWorkflowService
    {
        private readonly IWorkflowManager _workflowService = IoC.Resolve<IWorkflowManager>();
        private readonly IServicesRequestsTracer _requestTracer = IoC.Resolve<IServicesRequestsTracer>();
        
        [HttpGet]
        [Route("GetAllWorkflows")]
        [AllowAnonymous]
        public List<WFInfo> GetAllWorkflows()
        {
            DateTime startTime = DateTime.Now;
            try
            {
                var wfList = _workflowService.GetAllWorkflows();

                _requestTracer.TraceServiceRequestAsync(
                    this, Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds), null, wfList, 1, "");

                return wfList;
            }
            catch (Exception exc)
            {
                string errMsg = string.Format("Error get all workflows. Err: {0}", exc.ToString());
                Logger.Instance.Error(errMsg, LoggerConsts.WorkflowCreateWorkflow, exc);

                _requestTracer.TraceServiceRequestAsync(
                    this, Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds), null, null, 0, exc.ToString());

                throw;
            }
        }

        [HttpPost]
        [Route("GetWorkflowById")]
        [AllowAnonymous]
        public WFInfo GetWorkflowById(long wfId)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                var wf = _workflowService.GetWorkflowById(wfId);

                _requestTracer.TraceServiceRequestAsync(
                    this, Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds), new Object[] { wfId }, wf, 1, "");

                return wf;
            }
            catch (Exception exc)
            {
                string errMsg = string.Format("Error get workflow with <{0}> id. Err: {1}", wfId, exc.ToString());
                Logger.Instance.Error(errMsg, LoggerConsts.WorkflowCreateWorkflow, exc);

                _requestTracer.TraceServiceRequestAsync(
                    this, Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds), new Object[] { wfId }, null, 0, exc.ToString());

                throw;
            }
        }

        [HttpPost]
        [Route("GetWorkflowByIdentifier")]
        [AllowAnonymous]
        public WFInfo GetWorkflowByIdentifier(Guid wfIdentifier)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                var wf = _workflowService.GetWorkflowByIdentifier(wfIdentifier);

                _requestTracer.TraceServiceRequestAsync(
                    this, Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds), new Object[] { wfIdentifier }, wf, 1, "");

                return wf;
            }
            catch (Exception exc)
            {
                string errMsg = string.Format("Error get workflow with <{0}> identifier. Err: {1}", wfIdentifier, exc.ToString());
                Logger.Instance.Error(errMsg, LoggerConsts.WorkflowCreateWorkflow, exc);

                _requestTracer.TraceServiceRequestAsync(
                    this, Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds), new Object[] { wfIdentifier }, null, 0, exc.ToString());

                throw;
            }
        }

        [HttpPost]
        [Route("SaveWorkflow")]
        [AllowAnonymous]
        public WFInfo SaveWorkflow(WFInfo wf)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                wf = _workflowService.SaveWorkflow(wf);

                _requestTracer.TraceServiceRequestAsync(
                    this, Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds), new Object[] { wf }, wf, 1, "");

                return wf;
            }
            catch (Exception exc)
            {
                string errMsg = string.Format("Error save workflow with <{0}> name and <{1}> identifier. Err: {2}", wf.Name, wf.Identifier, exc.ToString());
                Logger.Instance.Error(errMsg, LoggerConsts.WorkflowSaveWorkflow, exc);

                _requestTracer.TraceServiceRequestAsync(
                    this, Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds), new Object[] { wf }, null, 0, exc.ToString());

                throw;
            }
        }

        [HttpPost]
        [Route("DeleteWorkflow")]
        [AllowAnonymous]
        public void DeleteWorkflow(WFInfo wf)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                _workflowService.DeleteWorkflow(wf);

                _requestTracer.TraceServiceRequestAsync(
                    this, Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds), new Object[] { wf }, null, 1, "");
            }
            catch (Exception exc)
            {
                string errMsg = string.Format("Error delete workflow with <{0}> id. Err: {1}", wf.Id, exc.ToString());
                Logger.Instance.Error(errMsg, LoggerConsts.WorkflowDeleteWorkflow, exc);

                _requestTracer.TraceServiceRequestAsync(
                    this, Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds), new Object[] { wf }, null, 0, exc.ToString());

                throw;
            }
        }



        [HttpPost]
        [Route("GetWorkflowInstanceById")]
        [AllowAnonymous]
        public WorkflowInstance GetWorkflowInstanceById(long wfInstanceId)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                var wfI = _workflowService.GetWFInstanceById(wfInstanceId);

                _requestTracer.TraceServiceRequestAsync(
                    this, Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds), new Object[] { wfInstanceId }, wfI, 1, "");

                return wfI;
            }
            catch (Exception exc)
            {
                string errMsg = string.Format("Error get workflow instance with <{0}> id. Err: {1}", wfInstanceId, exc.ToString());
                Logger.Instance.Error(errMsg, LoggerConsts.WorkflowCreateWorkflow, exc);

                _requestTracer.TraceServiceRequestAsync(
                    this, Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds), new Object[] { wfInstanceId }, null, 0, exc.ToString());

                throw;
            }
        }

        [HttpPost]
        [Route("GetWorkflowInstanceByCase")]
        [AllowAnonymous]
        public WorkflowInstance GetWorkflowInstanceByCase(CyberCase cyberCase)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                var wfI = _workflowService.GetWFInstanceByCase(cyberCase);

                _requestTracer.TraceServiceRequestAsync(
                    this, Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds), new Object[] { cyberCase }, wfI, 1, "");

                return wfI;
            }
            catch (Exception exc)
            {
                string errMsg = string.Format("Error get workflow instance with <{0}> case id. Err: {1}", ((null == cyberCase) ? 0 : cyberCase.Id), exc.ToString());
                Logger.Instance.Error(errMsg, LoggerConsts.WorkflowCreateWorkflow, exc);

                _requestTracer.TraceServiceRequestAsync(
                    this, Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds), new Object[] { cyberCase }, null, 0, exc.ToString());

                throw;
            }
        }

        //[HttpPost]
        //[Route("SaveWorkflowInstance")]
        //[AllowAnonymous]
        //public WorkflowInstance SaveWorkflowInstance(WorkflowInstance wfInstance)
        //{
        //    DateTime startTime = DateTime.Now;
        //    try
        //    {
        //        wfInstance = _wfMng.SaveWFInstance(wfInstance);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfInstance }, wfInstance, 1, "", Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        return wfInstance;
        //    }
        //    catch (Exception exc)
        //    {
        //        string errMsg = string.Format("Error save workflow instance with <{1}> case id and <{2}> WF name. Err: {3}", wfInstance.CaseId, wfInstance.WorkflowInfo.Name, exc.ToString());
        //        Logger.Instance.Error(errMsg, LoggerConsts.WFSaveWF, exc);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfInstance }, null, 0, exc.ToString(), Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        throw;
        //    }
        //}

        //#region Creat/Update methods
        //[HttpPost]
        //[Route("CreateWorkflow")]
        //public WFInfo CreateWorkflow(String name, String description)
        //{
        //    DateTime startTime = DateTime.Now;
        //    try
        //    {
        //        var wf = _wfMng.CreateWorkflow(name, description);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { name, description }, wf, 1, "", Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        return wf;
        //    }
        //    catch (Exception exc)
        //    {
        //        string errMsg = string.Format("Error create workflow with <{1}> name. Err: {2}", name, exc.ToString());
        //        Logger.Instance.Error(errMsg, LoggerConsts.WFCreateWF, exc);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { name, description }, null, 0, exc.ToString(), Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        throw;
        //    }
        //}
        //[HttpPost]
        //[Route("SaveWorkflow")]
        //public WFInfo SaveWorkflow(WFInfo wf)
        //{
        //    DateTime startTime = DateTime.Now;
        //    try
        //    {
        //        wf = _wfMng.SaveWorkflow(wf);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wf }, wf, 1, "", Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        return wf;
        //    }
        //    catch (Exception exc)
        //    {
        //        string errMsg = string.Format("Error save workflow with <{1}> name and <{2}> id. Err: {3}", wf.Name, wf.Id, exc.ToString());
        //        Logger.Instance.Error(errMsg, LoggerConsts.WFSaveWF, exc);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wf }, null, 0, exc.ToString(), Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        throw;
        //    }
        //}
        //[HttpPost]
        //[Route("SaveWorkflowStep")]
        //public WFStepInfo SaveWorkflowStep(WFStepInfo wfStep)
        //{
        //    DateTime startTime = DateTime.Now;
        //    try
        //    {
        //        wfStep = _wfMng.SaveWorkflowStep(wfStep);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfStep }, wfStep, 1, "", Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        return wfStep;
        //    }
        //    catch (Exception exc)
        //    {
        //        string errMsg = string.Format("Error save workflow step with <{1}> name and <{2}> id. Err: {3}", wfStep.StepName, wfStep.Id, exc.ToString());
        //        Logger.Instance.Error(errMsg, LoggerConsts.WFSaveWFStep, exc);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfStep }, null, 0, exc.ToString(), Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        throw;
        //    }
        //}
        //[HttpPost]
        //[Route("SaveWorkflowStepParameter")]
        //public WFStepParamInfo SaveWorkflowStepParameter(WFStepParamInfo wfStepParam)
        //{
        //    DateTime startTime = DateTime.Now;
        //    try
        //    {
        //        wfStepParam = _wfMng.SaveWorkflowStepParameter(wfStepParam);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfStepParam }, wfStepParam, 1, "", Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        return wfStepParam;
        //    }
        //    catch (Exception exc)
        //    {
        //        string errMsg = string.Format("Error save workflow step parameters with <{1}> name and <{2}> id. Err: {3}", wfStepParam.ParamName, wfStepParam.Id, exc.ToString());
        //        Logger.Instance.Error(errMsg, LoggerConsts.WFSaveWFStepParam, exc);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfStepParam }, null, 0, exc.ToString(), Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        throw;
        //    }
        //}
        //#endregion

        //#region Delete methods
        //[HttpPost]
        //[Route("DeleteWorkflow")]
        //public void DeleteWorkflow(long wfId)
        //{
        //    DateTime startTime = DateTime.Now;
        //    try
        //    {
        //        _wfMng.DeleteWorkflow(wfId);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfId }, null, 1, "", Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));
        //    }
        //    catch (Exception exc)
        //    {
        //        string errMsg = string.Format("Error delete workflow with <{1}> id. Err: {2}", wfId, exc.ToString());
        //        Logger.Instance.Error(errMsg, LoggerConsts.WFDeleteWF, exc);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfId }, null, 0, exc.ToString(), Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        throw;
        //    }
        //}
        //[HttpPost]
        //[Route("DeleteWorkflowStep")]
        //public void DeleteWorkflowStep(long wfStepId)
        //{
        //    DateTime startTime = DateTime.Now;
        //    try
        //    {
        //        _wfMng.DeleteWorkflowStep(wfStepId);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfStepId }, null, 1, "", Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));
        //    }
        //    catch (Exception exc)
        //    {
        //        string errMsg = string.Format("Error delete workflow step with <{1}> id. Err: {2}", wfStepId, exc.ToString());
        //        Logger.Instance.Error(errMsg, LoggerConsts.WFDeleteWFStep, exc);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfStepId }, null, 0, exc.ToString(), Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        throw;
        //    }
        //}
        //[HttpPost]
        //[Route("DeleteWorkflowStepParameters")]
        //public void DeleteWorkflowStepParameters(long wfStepParamId)
        //{
        //    DateTime startTime = DateTime.Now;
        //    try
        //    {
        //        _wfMng.DeleteWorkflowStepParameters(wfStepParamId);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfStepParamId }, null, 1, "", Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));
        //    }
        //    catch (Exception exc)
        //    {
        //        string errMsg = string.Format("Error delete workflow step parameter with <{1}> id. Err: {2}", wfStepParamId, exc.ToString());
        //        Logger.Instance.Error(errMsg, LoggerConsts.WFDeleteWFStepParam, exc);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfStepParamId }, null, 0, exc.ToString(), Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        throw;
        //    }
        //}
        //#endregion

        //#region Get by Id methods
        //[HttpGet]
        //[Route("GetWorkflowById")]
        //public WFInfo GetWorkflowById(long wfId)
        //{
        //    DateTime startTime = DateTime.Now;
        //    try
        //    {
        //        WFInfo wf = _wfMng.GetWorkflowById(wfId);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfId }, wf, 1, "", Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        return wf;
        //    }
        //    catch (Exception exc)
        //    {
        //        string errMsg = string.Format("Error get workflow with <{1}> id. Err: {2}", wfId, exc.ToString());
        //        Logger.Instance.Error(errMsg, LoggerConsts.WFGetWFById, exc);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfId }, null, 0, exc.ToString(), Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        throw;
        //    }
        //}
        //[HttpGet]
        //[Route("GetWorkflowStepById")]
        //public WFStepInfo GetWorkflowStepById(long wfStepId)
        //{
        //    DateTime startTime = DateTime.Now;
        //    try
        //    {
        //        WFStepInfo wfStep = _wfMng.GetWorkflowStepById(wfStepId);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfStepId }, wfStep, 1, "", Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        return wfStep;
        //    }
        //    catch (Exception exc)
        //    {
        //        string errMsg = string.Format("Error get workflow step with <{1}> id. Err: {2}", wfStepId, exc.ToString());
        //        Logger.Instance.Error(errMsg, LoggerConsts.WFGetWFStepById, exc);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfStepId }, null, 0, exc.ToString(), Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        throw;
        //    }
        //}
        //[HttpGet]
        //[Route("GetWorkflowStepParamById")]
        //public WFStepParamInfo GetWorkflowStepParamById(long wfStepParamId)
        //{
        //    DateTime startTime = DateTime.Now;
        //    try
        //    {
        //        WFStepParamInfo wfStepParam = _wfMng.GetWorkflowStepParamById(wfStepParamId);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfStepParamId }, wfStepParam, 1, "", Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        return wfStepParam;
        //    }
        //    catch (Exception exc)
        //    {
        //        string errMsg = string.Format("Error get workflow step parameter with <{1}> id. Err: {2}", wfStepParamId, exc.ToString());
        //        Logger.Instance.Error(errMsg, LoggerConsts.WFGetWFStepParamById, exc);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfStepParamId }, null, 0, exc.ToString(), Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        throw;
        //    }
        //}
        //#endregion

        //#region Get lists methods
        //[HttpGet]
        //[Route("GetAllWorkflows")]
        //public List<WFInfo> GetAllWorkflows()
        //{
        //    DateTime startTime = DateTime.Now;
        //    try
        //    {
        //        List<WFInfo> wfStepParam = _wfMng.GetAllWorkflows();

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, null, wfStepParam, wfStepParam.Count, "", Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        return wfStepParam;
        //    }
        //    catch (Exception exc)
        //    {
        //        string errMsg = string.Format("Error get all workflows. Err: {1}", exc.ToString());
        //        Logger.Instance.Error(errMsg, LoggerConsts.WFGetWFStepParamById, exc);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, null, null, 0, exc.ToString(), Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        throw;
        //    }
        //}
        //[HttpGet]
        //[Route("GetWFStepsByWFId")]
        //public List<WFStepInfo> GetWFStepsByWFId(long wfId)
        //{
        //    DateTime startTime = DateTime.Now;
        //    try
        //    {
        //        List<WFStepInfo> wfStep = _wfMng.GetWFStepsByWFId(wfId);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfId }, wfStep, wfStep.Count, "", Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        return wfStep;
        //    }
        //    catch (Exception exc)
        //    {
        //        string errMsg = string.Format("Error get all workflow steps by workflow id <{1}>. Err: {2}", wfId, exc.ToString());
        //        Logger.Instance.Error(errMsg, LoggerConsts.WFGetWFStepParamById, exc);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfId }, null, 0, exc.ToString(), Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        throw;
        //    }
        //}
        //[HttpGet]
        //[Route("GetWFStepParametersByWFStepId")]
        //public List<WFStepParamInfo> GetWFStepParametersByWFStepId(long wfStepId)
        //{
        //    DateTime startTime = DateTime.Now;
        //    try
        //    {
        //        List<WFStepParamInfo> wfStepParam = _wfMng.GetWFStepParametersByWFStepId(wfStepId);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfStepId }, wfStepParam, wfStepParam.Count, "", Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        return wfStepParam;
        //    }
        //    catch (Exception exc)
        //    {
        //        string errMsg = string.Format("Error get workflow steps parameters by workflow step id <{1}>. Err: {2}", wfStepId, exc.ToString());
        //        Logger.Instance.Error(errMsg, LoggerConsts.WFGetWFStepParamById, exc);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfStepId }, null, 0, exc.ToString(), Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        throw;
        //    }
        //}
        //#endregion

        //#region Get all data
        //[HttpGet]
        //[Route("GetAllWorkflowsFullData")]
        //public WorkflowsDescriptor GetAllWorkflowsFullData()
        //{
        //    DateTime startTime = DateTime.Now;
        //    try
        //    {
        //        WorkflowsDescriptor wfDescriptor = _wfMng.GetAllWorkflowsFullData();

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, null, wfDescriptor, 1, "", Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        return wfDescriptor;
        //    }
        //    catch (Exception exc)
        //    {
        //        string errMsg = string.Format("Error get workflow descriptor of all workflows. Err: {1}", exc.ToString());
        //        Logger.Instance.Error(errMsg, LoggerConsts.WFGetWFStepParamById, exc);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, null, null, 0, exc.ToString(), Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        throw;
        //    }
        //}
        //[HttpGet]
        //[Route("GetAllWorkflowFullData")]
        //public WorkflowsDescriptor GetAllWorkflowFullData(long wfId)
        //{
        //    DateTime startTime = DateTime.Now;
        //    try
        //    {
        //        WorkflowsDescriptor wfDescriptor = _wfMng.GetAllWorkflowFullData(wfId);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfId }, wfDescriptor, 1, "", Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        return wfDescriptor;
        //    }
        //    catch (Exception exc)
        //    {
        //        string errMsg = string.Format("Error get workflow descriptor for workflow id <{1}>. Err: {2}", wfId, exc.ToString());
        //        Logger.Instance.Error(errMsg, LoggerConsts.WFGetWFStepParamById, exc);

        //        _requestTracer.TraceServiceRequestAnsyc(
        //            this, new Object[] { wfId }, null, 0, exc.ToString(), Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds));

        //        throw;
        //    }
        //}
        //#endregion
    }

    
}
