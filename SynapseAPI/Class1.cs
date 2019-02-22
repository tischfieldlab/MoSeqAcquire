using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SynapseAPI
{
    public enum SynapseMode
    {
        Error = -1,
        Idle = 0,
        Standby = 1,
        Preview = 2,
        Record = 3
    }

    public class SynapseClient
    {
        private static HttpClient synClient;

        protected SynapseClient(string hostname, int port)
        {
            synClient.BaseAddress = new UriBuilder("http", hostname, port).Uri;
            synClient.DefaultRequestHeaders.Accept.Clear();
            synClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }


        public void connect() {
            try {
                synClient.connect();
            } catch (Exception e) {
                throw new Exception('failed to connect to Synapse\n' + e);
            }
        }
        protected async Task<T> Get<T>(string endpoint)
        {
            HttpResponseMessage response = await synClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody.Substring(0, 50) + "........");
            return JsonConvert.DeserializeObject<T>(responseBody);
        }
        protected async Task Put<T>(string endpoint, T data)
        {
            HttpResponseMessage response = await synClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody.Substring(0, 50) + "........");
            return JsonConvert.DeserializeObject<T>(responseBody);
        }

        public SynapseMode Mode
        {
            get
            {
                return this.Get<SynapseMode>("/system/mode").Result;
            }
        }

        public void getMode() { 
		    //'''
		    //-1: Error
		    // 0: Idle
		    // 1: Standby
		    // 2: Preview
		    // 3: Record
		    //'''


            try:
			    retval = this.Modes.index(this.sendGet('/system/mode', 'mode'))

            catch:
            retval = -1


            return retval
                }

        public getModeStr(this) :
		    '''
		    '' (Error)
		    'Idle'
		    'Standby'
		    'Preview'
		    'Record'
		    '''

		    retval = this.getMode()
		    if retval == -1:
			    retval = ''
		    else:
			    retval = this.Modes[retval]

		    return retval

        public setMode(mode) :
		    '''
		    mode must be an integer between 0 and 3, inclusive
		    '''

		    if mode in range(len(this.Modes)) :

                this.sendPut('/system/mode', json.dumps({'mode' : this.Modes[mode]
    }))
		    else:
			    raise Exception('invalid call to setMode()')


        public setModeStr(modeStr) :
		    '''
		    string equivalent of setMode()
		    '''

		    try:
			    mode = this.Modes.index(modeStr)
            except:
			    raise Exception('invalid call to setModeStr()')


            this.setMode(mode)

        public issueTrigger(id) :

            this.sendPut('/trigger/' + str(id), None)

	    public getSystemStatus(this) :

            retval = {'sysLoad' : 0, 'uiLoad' : 0, 'errorCount' : 0, 'rateMBps' : 0, 'recordSecs' : 0}
		    resp = this.sendGet('/system/status')

		    sysStat = {'sysLoad' : '', 'uiLoad' : '', 'errors' : '', 'dataRate' : '', 'recDur' : ''}
		    for key in resp:
			    try:
				    sysStat[key] = resp[key]
                except:
				    continue

		    // Synapse internal keys : user friendly keys
		    keyMap = {'sysLoad' : 'sysLoad', 'uiLoad' : 'uiLoad', 'errors' : 'errorCount', 'dataRate' : 'rateMBps', 'recDur' : 'recordSecs'}
		    for key in sysStat:
			    try:
				    if key == 'dataRate':
					    // '0.00 MB/s'
					    retval[keyMap[key]] = float (sysStat[key].split()[0])
				    else if key == 'recDur':
					    // 'HH:MM:SSs'
					    recDur = sysStat[key][:-1].split(':')

                        retval[keyMap[key]] = int (recDur[0]) * 3600 + int (recDur[1]) * 60 + int (recDur[2])
				    else:
					    retval[keyMap[key]] = int (sysStat[key])

                 except:
				    continue

		    return retval

        public getPersistModes(this) :
		    return this.parseJsonStringList(this.sendOptions('/system/persist', 'modes'))

	    public getPersistMode(this) :
		    return this.parseJsonString(this.sendGet('/system/persist', 'mode'))

	    public setPersistMode(modeStr) :

            this.sendPut('/system/persist', json.dumps({'mode' : modeStr}))

	    public getSamplingRates(this) :

            retval = {}
		    resp = this.sendGet('/processor/samprate')

		    for proc in list(resp.keys()) :

                retval[this.parseJsonString(proc)] = this.parseJsonFloat(resp[proc])

		    return retval

        public getKnownSubjects(this) :
		    return this.parseJsonStringList(this.sendOptions('/subject/name', 'subjects'))

	    public getKnownUsers(this) :
		    return this.parseJsonStringList(this.sendOptions('/user/name', 'users'))

	    public getKnownExperiments(this) :
		    return this.parseJsonStringList(this.sendOptions('/experiment/name', 'experiments'))

	    public getKnownTanks(this) :
		    return this.parseJsonStringList(this.sendOptions('/tank/name', 'tanks'))

	    public getKnownBlocks(this) :
		    return this.parseJsonStringList(this.sendOptions('/block/name', 'blocks'))

	    public getCurrentSubject(this) :
		    return this.parseJsonString(this.sendGet('/subject/name', 'subject'))

	    public getCurrentUser(this) :
		    return this.parseJsonString(this.sendGet('/user/name', 'user'))

	    public getCurrentExperiment(this) :
		    return this.parseJsonString(this.sendGet('/experiment/name', 'experiment'))

	    public getCurrentTank(this) :
		    return this.parseJsonString(this.sendGet('/tank/name', 'tank'))

	    public getCurrentBlock(this) :
		    return this.parseJsonString(this.sendGet('/block/name', 'block'))

	    public setCurrentSubject(name) :

            this.sendPut('/subject/name', json.dumps({'subject' : name}))

	    public setCurrentUser(name, pwd = '') :

            this.sendPut('/user/name', json.dumps({'user' : name, 'pwd' : pwd}))

	    public setCurrentExperiment(name) :

            this.sendPut('/experiment/name', json.dumps({'experiment' : name}))

	    public setCurrentTank(name) :

            this.sendPut('/tank/name', json.dumps({'tank' : name}))

	    public setCurrentBlock(name) :

            this.sendPut('/block/name', json.dumps({'block' : name}))

	    public createTank(path) :

            this.sendPut('/tank/path', json.dumps({'tank' : path}))

	    public createSubject(name, desc = '', icon = 'mouse') :

            this.sendPut('/subject/name/new', json.dumps({'subject' : name, 'desc' : desc, 'icon' : icon}))

	    public getGizmoNames(this) :
		    return this.parseJsonStringList(this.sendOptions('/gizmos', 'gizmos'))

	    public getParameterNames(gizmoName) :
		    return this.parseJsonStringList(this.sendOptions('/params/' + gizmoName, 'parameters'))

	    public getParameterInfo(gizmoName, paramName) :

            info = this.parseJsonStringList(this.sendGet('/params/info/%s.%s' % (gizmoName, paramName), 'info'))
		    keys = ('Name', 'Unit', 'Min', 'Max', 'Access', 'Type', 'Array')

		    retval = {}
		    for i in range(len(keys)) :

                key = keys[i]

			    try:
				    retval[key] = info[i]

				    if key == 'Array' and info[i] != 'No' and info[i] != 'Yes':
					    retval[key] = int (info[i])
                     else if key == 'Min' or key == 'Max':
					    retval[key] = float (info[i])

                 except:
				    retval[key] = None

		    return retval

        public getParameterSize(gizmoName, paramName) :
		    return this.parseJsonInt(this.sendGet('/params/size/%s.%s' % (gizmoName, paramName), 'value'))

	    public getParameterValue(gizmoName, paramName) :

            value = this.sendGet('/params/%s.%s' % (gizmoName, paramName), 'value')

		    didConvert = [True]
    retval = this.parseJsonFloat(value, didConvert)
		
		    if not didConvert[0]:
			    retval = this.parseJsonString(value)

		    return retval

        public getParameterValues(gizmoName, paramName, count = -1, offset = 0)
{
    '''
		    if count == -1 {
        count = getParameterSize(gizmoName, paramName)
		    '''

		    if count == -1 {
            try:
				    count = this.getParameterSize(gizmoName, paramName)
                except:
				    count = 1

		    values = this.sendGet('/params/%s.%s' % (gizmoName, paramName),
							      'values',
							      json.dumps({'count' : count, 'offset' : offset}))

		    // HACK to pass variable by reference
		    didConvert = [True]
    retval = this.parseJsonFloatList(values, didConvert)
		
		    if not didConvert[0]:
			    retval = this.parseJsonStringList(values)
			
		    return retval[:min(count, len(retval))]

        public setParameterValue(gizmoName, paramName, value)
{

    this.sendPut('/params/%s.%s' % (gizmoName, paramName), json.dumps({'value' : value}))

	    public setParameterValues(gizmoName, paramName, values, offset = 0)
{

    this.sendPut('/params/%s.%s' % (gizmoName, paramName), json.dumps({'offset' : offset, 'values' : values}))

	    public appendExperimentMemo(experiment, memo)
{

    this.sendPut('/experiment/notes', json.dumps({'experiment' : experiment, 'memo' : memo}))

	    public appendSubjectMemo(subject, memo) { 

            this.sendPut('/subject/notes', json.dumps({'subject' : subject, 'memo' : memo}))

	    public appendUserMemo(user, memo) {
        this.sendPut('/user/notes', json.dumps({ 'user' : user, 'memo' : memo}))
        }

	    public startDemo(name) :
		    if name not in this.demoExperiments:
			    raise Exception('%s is not a valid demo experiment' % name)
		    if this.getCurrentExperiment() != name:
			    if name not in this.getKnownExperiments():
				    raise Exception('Experiment %s not found' % name)
			    if this.getModeStr() != 'Idle':
			       this.setModeStr('Idle')
			    try:
				    this.setCurrentExperiment(name)
                except:
				    raise Exception('Experiment %s not selected' % name)

		    if this.demoRequiredGizmos[name] not in this.getGizmoNames():
			    raise Exception('Required gizmo %s not found' % this.demoRequiredGizmos[name])

		    if this.getModeStr() == 'Idle':
			    this.setPersistMode('Fresh')
			    this.setModeStr('Record')



    }
}
