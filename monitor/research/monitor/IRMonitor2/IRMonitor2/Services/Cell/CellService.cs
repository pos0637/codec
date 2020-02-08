using Common;
using Devices;
using Repository.Entities;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IRMonitor2.Services.Cell
{
    /// <summary>
    /// 设备单元服务
    /// </summary>
    public partial class CellService : Service
    {
        #region 参数

        /// <summary>
        /// 设备单元配置
        /// </summary>
        private Configuration.Cell mCell;

        /// <summary>
        /// 选区列表
        /// </summary>
        private List<Selection> mSelections = new List<Selection>();

        /// <summary>
        /// 组选区列表
        /// </summary>
        private List<SelectionGroup> mSelectionGroups = new List<SelectionGroup>();

        /// <summary>
        /// 设备列表
        /// </summary>
        private List<IDevice> mDeviceList = new List<IDevice>();

        private byte[] mRealtimeImage; // 实时图像缓存        
        /*
        private long mCurrentRecordId = -1; // 告警录像索引
        private AlarmNoticeConfig mAlarmNoticeConfig; // 告警通知
        private TempCurveSample mTempCurveSample; // 采样频率
        private UserRoutineConfig mUserRoutineConfig; // 用户常规设置
        private DeviceInfo mDeviceInfo; // 设备信息
        private IRParam mIRParam; // 红外参数

        private RealtimeNoticeWorker mRealtimeNoticeWorker = new RealtimeNoticeWorker(); // 实时通知
        private GetIrDataWorker mGetIrDataWorker = new GetIrDataWorker(); // 数据获取线程
        private ProcessingWorker mProcessingWorker = new ProcessingWorker(); // 温度告警线程
        private RecordingWorker mRecordingWorker = new RecordingWorker(); // 录像线程
        */
        #region 转接口
        /*
        public event Delegates.DgOnImageCallback OnImageCallback {
            add { mGetIrDataWorker.OnImageCallback += value; }
            remove { mGetIrDataWorker.OnImageCallback -= value; }
        }

        public event Delegates.DgOnTemperatureCallback OnTempertureCallback {
            add { mGetIrDataWorker.OnTemperatureCallback += value; }
            remove { mGetIrDataWorker.OnTemperatureCallback -= value; }
        }

        public event Delegates.DgOnRealtimeSelectionTemperature OnRealtimeSelectionTemperature {
            add { mProcessingWorker.OnRealtimeSelectionTemperature += value; }
            remove { mProcessingWorker.OnRealtimeSelectionTemperature -= value; }
        }

        public event Delegates.DgOnReportDataCallback OnReportDataCallback;

        public event Delegates.DgOnGetTempCurve OnGetTempCurve;

        public event Delegates.DgOnGetTempCurve OnRealTemperatureInfo;
        */
        #endregion

        #endregion

        public override void Initialize(Dictionary<string, object> arguments)
        {
            #region 所有参数初始化

            mCell = arguments["cell"] as Configuration.Cell;

            #endregion

            base.Initialize(arguments);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public ARESULT Initialize(Models.Cell cell)
        {
            #region 所有参数初始化

            mCell = cell;

            if (ARESULT.AFAILED(LoadAlarmNoticeConfigInfo())) {
                Tracker.LogI("LoadAlarmNoticeConfigInfo FAILED");
                return ARESULT.E_FAIL;
            }

            if (ARESULT.AFAILED(LoadCurveSample())) {
                Tracker.LogI("LoadCurveSample FAILED");
                return ARESULT.E_FAIL;
            }

            if (ARESULT.AFAILED(LoadUserRotineInfo())) {
                Tracker.LogI("LoadUserRotineInfo FAILED");
                return ARESULT.E_FAIL;
            }

            if (ARESULT.AFAILED(LoadDeviceInfo())) {
                Tracker.LogI("LoadDeviceInfo FAILED");
                return ARESULT.E_FAIL;
            }

            if (ARESULT.AFAILED(LoadIRParam())) {
                Tracker.LogI("LoadIRParam FAILED");
                return ARESULT.E_FAIL;
            }

            if (ARESULT.AFAILED(FixAlarmInfo())) {
                Tracker.LogI("FixAlarmInfo FAILED");
                return ARESULT.E_FAIL;
            }

            if (ARESULT.AFAILED(FixSelection())) {
                Tracker.LogI("FixSelection FAILED");
                return ARESULT.E_FAIL;
            }

            if (ARESULT.AFAILED(LoadSelections())) {
                Tracker.LogI("LoadSelectionList FAILED");
                return ARESULT.E_FAIL;
            }

            if (ARESULT.AFAILED(LoadGroupSelectionList())) {
                Tracker.LogI("LoadSelectionList FAILED");
                return ARESULT.E_FAIL;
            }

            #endregion

            #region 事件与槽连接
            /*
            // 保存一份实时的图像数据，用于生产告警图像
            OnImageCallback += delegate (byte[] buf) { mRealtimeImage = buf; };

            // 录像
            mRecordingWorker.OnAddRecord += new Delegates.DgOnAddRecordInfo(AddRecord);

            // 温度处理
            mProcessingWorker.SetSelctionSampleRate(mTempCurveSample.mSelectionSample);
            mProcessingWorker.SetGroupSelctionSampleRate(mTempCurveSample.mSelectionSample);
            OnTempertureCallback += new Delegates.DgOnTemperatureCallback(mProcessingWorker.ReceiveTempertureData);
            mProcessingWorker.OnAddAlarmInfo += new Delegates.DgOnAddAlarmInfo(AddAlarmInfo);
            mProcessingWorker.OnUpdateAlarmInfo += new Delegates.DgOnUpdateAlarmInfo(UpdateAlarmInfo);
            mProcessingWorker.OnAlarmTimeout += new Delegates.DgOnAlarmTimeout(AlarmTimeout);
            mProcessingWorker.OnAddSelectionTemperature += new Delegates.DgOnAddSelectionTemperature(AddSelectionTemperature);
            mProcessingWorker.OnAddGroupSelectionTemperature += new Delegates.DgOnAddGroupSelectionTemperature(AddGroupSelectionTemperature);
            mProcessingWorker.OnSendReportData += new Delegates.DgOnSendReportData(SendReportData);
            mProcessingWorker.OnSendRealTemperature += new Delegates.DgOnSendRealTemperature(SendRealTemperatureInfo);

            // 定时和整点
            mRealtimeNoticeWorker.SetConfig(mAlarmNoticeConfig.mIsHourSend, mAlarmNoticeConfig.mIsRegularTimeSend, mAlarmNoticeConfig.mRegularTime);
            mRealtimeNoticeWorker.OnHourSend += new Delegates.DgOnHourSend(SendShortMessageCallback);
            mRealtimeNoticeWorker.OnRegularSend += new Delegates.DgOnRegularSend(SendShortMessageCallback);

            // 短信自动回复
            SmsServiceWorker.Instance.gOnReceiveShortMessage += new Delegates.DgOnAutoReplyShortMessage(OnAutoReplyShortMessageCallback);
            */
            #endregion

            return ARESULT.S_OK;
        }

        /// <summary>
        /// 开始
        /// </summary>
        /// <returns>是否成功</returns>
        public ARESULT Start()
        {
            // 加载设备
            IDevice device = DeviceFactory.Instance.GetDevice(0, mCell.mIRCameraType, "红外设备");
            if (device == null) {
                Tracker.LogI(string.Format("Load ({0}) Device FAILED", mCell.mIRCameraType));
                return ARESULT.E_FAIL;
            }

            Tracker.LogI(string.Format("Load ({0}) Device SUCCEED", mCell.mIRCameraType));

            lock (mDeviceList) {
                mDeviceList.Add(device);
            }

            // 设置红外参数
            SetEmissivity(mIRParam.mEmissivity);
            SetReflectedTemperature(mIRParam.mReflectedTemperature);
            SetTransmission(mIRParam.mTransmission);
            SetAtmosphericTemperature(mIRParam.mAtmosphericTemperature);
            SetRelativeHumidity(mIRParam.mRelativeHumidity);
            SetDistance(mIRParam.mDistance);

            // 数据获取线程初始化
            mGetIrDataWorker.Initialize(mCell.mIRCameraWidth, mCell.mIRCameraHeight, mCell.mIRCameraIp, mCell.mIRCameraVideoFrameRate, mCell.mIRCameraTemperatureFrameRate, device);

            // 录像线程初始化
            mRecordingWorker.Initialize(this);

            // 温度处理线程初始化
            mProcessingWorker.Initialize(mCell.mIRCameraWidth, mCell.mIRCameraHeight, mSelections, mSelectionGroups);

            // 开启所有线程
            if (ARESULT.AFAILED(mGetIrDataWorker.Start())) {
                Tracker.LogI("GetIrDataWorker Start FAILED");
                return ARESULT.E_FAIL;
            }

            if (ARESULT.AFAILED(mProcessingWorker.Start())) {
                Tracker.LogI("ProcessingWorker Start FAILED");
                return ARESULT.E_FAIL;
            }

            if (ARESULT.AFAILED(mRealtimeNoticeWorker.Start())) {
                Tracker.LogI("RealtimeNoticeWorker Start FAILED");
                return ARESULT.E_FAIL;
            }

            return ARESULT.S_OK;
        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <returns>是否成功</returns>
        public ARESULT Finalize()
        {
            SmsServiceWorker.Instance.gOnReceiveShortMessage -= new Delegates.DgOnAutoReplyShortMessage(OnAutoReplyShortMessageCallback);

            if (mGetIrDataWorker != null) {
                mGetIrDataWorker.Discard();
                mGetIrDataWorker.Join();
            }

            if (mProcessingWorker != null) {
                mProcessingWorker.Discard();
                mProcessingWorker.Join();
            }

            if (mRealtimeNoticeWorker != null) {
                mRealtimeNoticeWorker.Discard();
                mRealtimeNoticeWorker.Join();
            }

            if (mRecordingWorker != null) {
                mRecordingWorker.Discard();
                mRecordingWorker.Join();
            }

            foreach (IDevice item in mDeviceList) {
                item.Close();
            }

            return ARESULT.S_OK;
        }

        /// <summary>
        /// 创建选区
        /// </summary>
        /// <param name="type">选区类型</param>
        /// <returns>选区</returns>
        private Selection CreateSelection(SelectionType type)
        {
            switch (type) {
                case SelectionType.Point:
                    return new PointSelection();
                case SelectionType.Rectangle:
                    return new RectangleSelection();
                case SelectionType.Ellipse:
                    return new EllipseSelection();
                case SelectionType.Line:
                    return new LineSelection();
                default:
                    return null;
            }
        }

        /// <summary>
        /// 创建选区
        /// </summary>
        /// <param name="type">选区类型</param>
        /// <returns>选区</returns>        
        private Selection GetSelection(long id)
        {
            lock (mSelections) {
                return mSelections.Find(selection => selection.mSelectionId == id);
            }
        }

        #region 选区列表

        /// <summary>
        /// 加载选区列表
        /// </summary>
        /// <returns>是否成功</returns>
        private ARESULT LoadSelections()
        {
            try {
                var selections = Repository.Repository.LoadSelections();

                List<string> strList = SelectionDAO.GetSelectionList(mCell.mCellId);
                if (strList != null) {
                    foreach (string str in strList)
                        AddSelectionList(str);
                }

                // 检查是否有全局选区
                bool isAdd = true;
                lock (mSelections) {
                    foreach (Selection selection in mSelections) {
                        if (selection.mIsGlobalSelection) {
                            isAdd = false;
                            break;
                        }
                    }
                }

                if (isAdd) {
                    // 补充添加全局选区
                    var selection = new RectangleSelection {
                        mIsGlobalSelection = true,
                        mRectangle = new Rectangle(0, 0, mCell.mIRCameraWidth, mCell.mIRCameraHeight)
                    };
                    selection.MakeSelectionArea();

                    long id = -1;
                    if (ARESULT.AFAILED(SelectionDAO.AddSelection(mCell.mCellId, selection.Serialize(), true, ref id))) {
                        return ARESULT.E_FAIL;
                    }

                    lock (mSelections) {
                        mSelections.Add(selection.SetId(id));
                    }
                }

                return ARESULT.S_OK;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return ARESULT.E_FAIL;
            }
        }

        /// <summary>
        /// 添加选区列表
        /// </summary>
        /// <param name="selections">选区列表</param>
        /// <returns>是否成功</returns>
        private ARESULT AddSelections(List<Selections.Selection> selections)
        {
            int pos = selectionData.IndexOf(",");
            if (pos < 0) {
                return ARESULT.E_FAIL;
            }

            string str = selectionData.Substring(0, pos);
            if (string.IsNullOrEmpty(str)) {
                return ARESULT.E_FAIL;
            }

            long id;
            if (!long.TryParse(str, out id)) {
                return ARESULT.E_FAIL;
            }

            selectionData = selectionData.Substring(pos + 1);
            SelectionType type = Selection.GetType(selectionData);
            if (type == SelectionType.Unknown) {
                return ARESULT.E_INVALIDARG;
            }

            Selection selection;
            switch (type) {
                case SelectionType.Point:
                    selection = new PointSelection();
                    break;
                case SelectionType.Rectangle:
                    selection = new RectangleSelection();
                    break;
                case SelectionType.Ellipse:
                    selection = new EllipseSelection();
                    break;
                case SelectionType.Line:
                    selection = new LineSelection();
                    break;

                default:
                    return ARESULT.E_INVALIDARG;
            }

            if (ARESULT.AFAILED(selection.Deserialize(selectionData))) {
                return ARESULT.E_INVALIDARG;
            }

            lock (mSelections) {
                mSelections.Add(selection.SetId(id));
            }

            return ARESULT.S_OK;
        }

        /// <summary>
        /// 添加选区列表
        /// </summary>
        /// <param name="groupSelectionData">所有的选区信息</param>
        /// <returns>是否成功</returns>
        private ARESULT AddGroupSelectionList(
            string groupSelectionData)
        {
            int pos = groupSelectionData.IndexOf(",");
            if (pos < 0)
                return ARESULT.E_FAIL;

            string str = groupSelectionData.Substring(0, pos);
            if (string.IsNullOrEmpty(str))
                return ARESULT.E_FAIL;

            long id = -1;
            if (!long.TryParse(str, out id))
                return ARESULT.E_FAIL;

            groupSelectionData = groupSelectionData.Substring(pos + 1);
            SelectionGroup groupSelection = new SelectionGroup();
            if (ARESULT.AFAILED(groupSelection.Deserialize(groupSelectionData)))
                return ARESULT.E_FAIL;

            groupSelection.mId = id;
            groupSelection.mTemperatureData.mGroupId = id;

            lock (mSelectionGroups) {
                mSelectionGroups.Add(groupSelection);
            }

            return ARESULT.S_OK;
        }

        #endregion

        #region 摄像头操作        

        /// <summary>
        /// 获取大气温度
        /// </summary>
        /// <param name="temperature">大气温度</param>
        /// <returns>是否成功</returns>
        public ARESULT GetAtmosphericTemperature(ref float temperature)
        {
            return GetIrCameraDevice()?.Read(ReadMode.AtmosphericTemperature, temperature, out int used) ?? false
                ? ARESULT.S_OK : ARESULT.E_FAIL;
        }

        /// <summary>
        /// 设置大气温度
        /// </summary>
        /// <param name="temperature">大气温度</param>
        /// <returns>是否成功</returns>
        public ARESULT SetAtmosphericTemperature(float temperature)
        {
            return GetIrCameraDevice()?.Write(WriteMode.AtmosphericTemperature, temperature) ?? false
                ? ARESULT.S_OK : ARESULT.E_FAIL;
        }

        /// <summary>
        /// 获取相对湿度
        /// </summary>
        /// <param name="relativeHumidity">相对湿度</param>
        /// <returns>是否成功</returns>
        public ARESULT GetRelativeHumidity(ref float relativeHumidity)
        {
            return GetIrCameraDevice()?.Read(ReadMode.RelativeHumidity, relativeHumidity, out int used) ?? false
                ? ARESULT.S_OK : ARESULT.E_FAIL;
        }

        /// <summary>
        /// 设置相对湿度
        /// </summary>
        /// <param name="relativeHumidity">相对湿度</param>
        /// <returns>是否成功</returns>
        public ARESULT SetRelativeHumidity(float relativeHumidity)
        {
            return GetIrCameraDevice()?.Write(WriteMode.RelativeHumidity, relativeHumidity) ?? false
                ? ARESULT.S_OK : ARESULT.E_FAIL;
        }

        /// <summary>
        /// 获取相对温度
        /// </summary>
        /// <param name="reflectedTemperature">相对温度值</param>
        /// <returns>是否成功</returns>
        public ARESULT GetReflectedTemperature(ref float reflectedTemperature)
        {
            return GetIrCameraDevice()?.Read(ReadMode.ReflectedTemperature, reflectedTemperature, out int used) ?? false
                ? ARESULT.S_OK : ARESULT.E_FAIL;
        }

        /// <summary>
        /// 设置相对温度
        /// </summary>
        /// <param name="reflectedTemperature"></param>
        /// <returns>是否成功</returns>
        public ARESULT SetReflectedTemperature(float reflectedTemperature)
        {
            return GetIrCameraDevice()?.Write(WriteMode.ReflectedTemperature, reflectedTemperature) ?? false
                ? ARESULT.S_OK : ARESULT.E_FAIL;
        }

        /// <summary>
        /// 获取距离
        /// </summary>
        /// <param name="distance">距离值</param>
        /// <returns>是否成功</returns>
        public ARESULT GetDistance(ref float distance)
        {
            return GetIrCameraDevice()?.Read(ReadMode.ObjectDistance, distance, out int used) ?? false
                ? ARESULT.S_OK : ARESULT.E_FAIL;
        }

        /// <summary>
        /// 设置距离值
        /// </summary>
        /// <param name="distance">距离值</param>
        /// <returns>是否成功</returns>
        public ARESULT SetDistance(float distance)
        {
            return GetIrCameraDevice()?.Write(WriteMode.ObjectDistance, distance) ?? false
                ? ARESULT.S_OK : ARESULT.E_FAIL;
        }

        /// <summary>
        /// 获取辐射率
        /// </summary>
        /// <param name="emissivity">辐射率</param>
        /// <returns>是否成功</returns>
        public ARESULT GetEmissivity(ref float emissivity)
        {
            return GetIrCameraDevice()?.Read(ReadMode.Emissivity, emissivity, out int used) ?? false
                ? ARESULT.S_OK : ARESULT.E_FAIL;
        }

        /// <summary>
        ///  设置辐射率
        /// </summary>
        /// <param name="emissivity">辐射率</param>
        /// <returns>是否成功</returns>
        public ARESULT SetEmissivity(float emissivity)
        {
            return GetIrCameraDevice()?.Write(WriteMode.Emissivity, emissivity) ?? false
                ? ARESULT.S_OK : ARESULT.E_FAIL;
        }

        /// <summary>
        /// 获取透射
        /// </summary>
        /// <param name="transmission">透射</param>
        /// <returns>是否成功</returns>
        public ARESULT GetTransmission(ref float transmission)
        {
            return GetIrCameraDevice()?.Read(ReadMode.Transmission, transmission, out int used) ?? false
                ? ARESULT.S_OK : ARESULT.E_FAIL;
        }

        /// <summary>
        /// 设置透射
        /// </summary>
        /// <param name="transmission">透射</param>
        /// <returns>是否成功</returns>
        public ARESULT SetTransmission(float transmission)
        {
            return GetIrCameraDevice()?.Write(WriteMode.Transmission, transmission) ?? false
                ? ARESULT.S_OK : ARESULT.E_FAIL;
        }

        /// <summary>
        /// 自动对焦
        /// </summary>
        /// <returns>是否成功</returns>
        public ARESULT IrCameraAutoFocus()
        {
            return GetIrCameraDevice()?.Control(ControlMode.AutoFocus) ?? false
                ? ARESULT.S_OK : ARESULT.E_FAIL;
        }

        /// <summary>
        /// 远焦
        /// </summary>
        /// <returns>是否成功</returns>
        public ARESULT IrCameraFocusFar()
        {
            return GetIrCameraDevice()?.Control(ControlMode.FocusFar) ?? false
                ? ARESULT.S_OK : ARESULT.E_FAIL;
        }

        /// <summary>
        /// 近焦
        /// </summary>
        /// <returns>是否成功</returns>
        public ARESULT IrCameraFocusNear()
        {
            return GetIrCameraDevice()?.Control(ControlMode.FocusNear) ?? false
                ? ARESULT.S_OK : ARESULT.E_FAIL;
        }

        /// <summary>
        /// 设置采样率
        /// </summary>
        /// <param name="rate">采样率</param>
        /// <returns>是否成功</returns>
        public ARESULT SetDeviceSampleRate(int rate)
        {
            return GetIrCameraDevice()?.Write(WriteMode.FrameRate, BitConverter.GetBytes(rate)) ?? false
                ? ARESULT.S_OK : ARESULT.E_FAIL;
        }

        #endregion

        #region 录像

        /// <summary>
        /// 开启手动录像
        /// </summary>
        /// <returns></returns>
        public ARESULT StartManualRecord(ref long recordId)
        {
            mRecordingWorker.Start();

            long id = -1;
            if (ARESULT.AFAILED(AddManualRecord(ref id))) {
                mRecordingWorker.Stop();
                return ARESULT.E_FAIL;
            }

            recordId = id;

            return ARESULT.S_OK;
        }

        /// <summary>
        /// 关闭手动录像
        /// </summary>
        /// <returns></returns>
        public ARESULT StopManualRecord(long recordId)
        {
            mRecordingWorker.Stop();

            if (ARESULT.AFAILED(UpdateManualRecord(recordId))) {
                return ARESULT.E_FAIL;
            }

            return ARESULT.S_OK;
        }

        /// <summary>
        /// 添加告警录像
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        public void AddRecord(string filename)
        {
            long id = -1;
            if (!ARESULT.AFAILED(RecordDAO.AddRecord(mCell.mCellId, filename, null, ref id))) {
                mCurrentRecordId = id;
            }
        }

        /// <summary>
        /// 添加手动录像
        /// </summary>
        /// <returns></returns>
        public ARESULT AddManualRecord(ref long recordId)
        {
            long id = -1;
            if (ARESULT.AFAILED(ManualRecordDAO.AddManualRecord(mCell.mCellId, mCurrentRecordId, ref id)))
                return ARESULT.E_FAIL;
            else {
                recordId = id;
                return ARESULT.S_OK;
            }
        }

        /// <summary>
        /// 更新手动录像
        /// </summary>
        /// <returns></returns>
        public ARESULT UpdateManualRecord(long recordId)
        {
            return ManualRecordDAO.UpdateManualRecord(recordId, mCurrentRecordId);
        }

        #endregion

        #region 用户常规信息

        /// <summary>
        /// 加载用户常规信息
        /// </summary>
        public ARESULT LoadUserRotineInfo()
        {
            UserRoutineConfig routine = UserRotineDAO.GetUserRotineInfo();
            if (routine == null)
                return ARESULT.E_FAIL;

            mUserRoutineConfig = routine;
            return ARESULT.S_OK;
        }

        /// <summary>
        /// 获取用户常规信息
        /// </summary>
        public ARESULT GetUserRotineInfo(ref string str)
        {
            str = JsonUtils.ObjectToJson(mUserRoutineConfig);
            if (str != null)
                return ARESULT.S_OK;
            else
                return ARESULT.E_FAIL;
        }

        /// <summary>
        /// 更新用户常规信息
        /// </summary>
        public ARESULT UpdateUserRoutineInfo(
            string data)
        {
            UserRoutineConfig config = JsonUtils.ObjectFromJson<UserRoutineConfig>(data);
            if (config == null)
                return ARESULT.E_FAIL;

            int id = 0;
            ARESULT ret = UserRotineDAO.UpdateUserRoutineInfo(
                config.mProvince,
                config.mCity,
                config.mCompany,
                config.mProjectLeader,
                config.mTestPersonnel,
                config.mSubstation,
                config.mDevicePosition,
                ref id);

            if (ARESULT.AFAILED(ret))
                return ARESULT.E_FAIL;

            bool isSend = false;
            lock (mUserRoutineConfig) {
                if (!mUserRoutineConfig.mProvince.Equals(config.mProvince) || !mUserRoutineConfig.mCity.Equals(config.mCity)) {
                    isSend = true;
                }

                mUserRoutineConfig.mId = id;
                mUserRoutineConfig.mCity = config.mCity;
                mUserRoutineConfig.mCompany = config.mCompany;
                mUserRoutineConfig.mDevicePosition = config.mDevicePosition;
                mUserRoutineConfig.mProjectLeader = config.mProjectLeader;
                mUserRoutineConfig.mProvince = config.mProvince;
                mUserRoutineConfig.mSubstation = config.mSubstation;
                mUserRoutineConfig.mTestPersonnel = config.mTestPersonnel;
            }

            if (isSend) {
                SendReport();
            }

            return ARESULT.S_OK;
        }

        #endregion

        #region 告警

        /// <summary>
        /// 添加告警记录
        /// </summary>
        public ARESULT AddAlarmInfo(
            int alarmMode,
            int alarmType,
            int alarmReason,
            int alarmLevel,
            string alarmCondition,
            float alarmTemperature,
            long selectionId,
            string selectionName,
            string selectionData,
            string temperatureInfo,
            float maxTemperature,
            float minTemperature,
            List<Selection> selections,
            AlarmInfo info)
        {
            try {
                string imagePath = string.Format("{0}/{1}.JPG", mCell.mIRCameraImageFolder, DateTime.Now.ToString("yyyyMMddHHmmss"));
                string jstr = JsonUtils.ObjectToJson(mUserRoutineConfig);
                if (jstr == null) {
                    return ARESULT.E_FAIL;
                }

                // 生成告警图片
                byte[] data = mRealtimeImage;

                bool ret = ImageGenerator.CreateImage(
                    data,
                    selections,
                    mCell.mIRCameraWidth,
                    mCell.mIRCameraHeight,
                    maxTemperature,
                    minTemperature,
                    imagePath,
                    jstr,
                    IRPalette.PaletteType.Red);
                if (!ret)
                    return ARESULT.E_FAIL;

                // 未开启告警录像，录像起始Id为 -1
                long recordId = -1;
                info.mIsRecord = false;
                if (((alarmMode == (int)AlarmMode.Selection) && mAlarmNoticeConfig.mIsSelectionRecord)
                    || ((alarmMode == (int)AlarmMode.GroupSelection) && mAlarmNoticeConfig.mIsGroupSelectionRecord)) {
                    mRecordingWorker.Start();
                    recordId = mCurrentRecordId;
                    info.mIsRecord = true;
                }

                // 拼接告警详情
                string alarmDatail = AlarmMsgBuilder.GetAlarmDetail(
                    selectionName,
                    alarmTemperature,
                    alarmMode,
                    alarmType,
                    alarmReason,
                    alarmLevel,
                    alarmCondition);

                // 告警入库
                ARESULT result = AlarmDAO.AddAlarmInfo(
                   mCell.mCellId,
                   alarmMode,
                   alarmType,
                   alarmReason,
                   alarmLevel,
                   alarmDatail,
                   mCell.mIRCameraWidth,
                   mCell.mIRCameraHeight,
                   selectionId,
                   selectionData,
                   temperatureInfo,
                   imagePath,
                   alarmCondition,
                   alarmTemperature,
                   recordId,
                   mUserRoutineConfig.mId,
                   mIRParam.mId,
                   info.mIsRecord);
                if (ARESULT.AFAILED(result))
                    return ARESULT.E_FAIL;

                // 告警短信
                if (mAlarmNoticeConfig.mIsAlarmSend) {
                    string content = AlarmMsgBuilder.GetShowMessage(
                        mUserRoutineConfig.mSubstation,
                        mUserRoutineConfig.mDevicePosition,
                        alarmMode,
                        alarmType,
                        alarmLevel,
                        alarmTemperature,
                        DateTime.Now);

                    SmsServiceWorker.Instance.SendMessage(mAlarmNoticeConfig.mSendUser, content);
                }

                // 推送报表数据
                ReportData reportData = new ReportData {
                    mType = alarmMode,
                    mId = selectionId,
                    mProvince = mUserRoutineConfig.mProvince,
                    mCity = mUserRoutineConfig.mCity,
                    mDeviceName = mUserRoutineConfig.mDevicePosition,
                    mReportType = 0,
                    mUnit = mUserRoutineConfig.mCompany,
                    mTestPersonName = mUserRoutineConfig.mTestPersonnel,
                    mChargePersonName = mUserRoutineConfig.mProjectLeader,
                    mSubstation = mUserRoutineConfig.mSubstation,
                    mImageData = Convert.ToBase64String(Utils.GetBytesByImagePath(imagePath)),
                    mDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    mIRParam = mIRParam,
                    mDeviceInfo = mDeviceInfo
                };
                OnReportDataCallback?.Invoke(reportData);

                return ARESULT.S_OK;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return ARESULT.E_FAIL;
            }
        }

        /// <summary>
        /// 更新告警信息
        /// </summary>
        /// <returns>是否成功</returns>
        public ARESULT UpdateAlarmInfo(
            int alarmMode,
            int alarmType,
            long selectionId,
            string selectionData,
            AlarmInfo alarmInfo)
        {
            if (!alarmInfo.mIsTimeout) {
                AlarmTimeout(
                    alarmMode,
                    alarmType,
                    selectionId,
                    selectionData,
                    alarmInfo);
            }

            ARESULT ret = AlarmDAO.UpdateAlarmInfo(
                mCell.mCellId,
                selectionId,
                alarmMode,
                alarmType);
            if (ARESULT.AFAILED(ret))
                return ARESULT.E_FAIL;

            return ARESULT.S_OK;
        }

        /// <summary>
        /// 告警超时, 如果有录像则停止录像
        /// </summary>
        public ARESULT AlarmTimeout(
            int alarmMode,
            int alarmType,
            long selectionId,
            string selectionData,
            AlarmInfo alarmInfo)
        {
            // 未开启告警录像，录像起始Id为 -1
            long recordId = -1;
            if (alarmInfo.mIsRecord) {
                mRecordingWorker.Stop();
                recordId = mCurrentRecordId;
                alarmInfo.mIsRecord = false;
            }

            ARESULT ret = AlarmDAO.UpdateAlarmRecord(
                mCell.mCellId,
                selectionId,
                recordId,
                alarmMode,
                alarmType);
            if (ARESULT.AFAILED(ret))
                return ARESULT.E_FAIL;

            return ARESULT.S_OK;
        }

        #endregion

        #region 告警通知

        /// <summary>
        /// 加载告警通知配置信息
        /// </summary>
        public ARESULT LoadAlarmNoticeConfigInfo()
        {
            AlarmNoticeConfig alarmConfig = AlarmNoticeConfigDAO.GetAlarmNoticeConfigInfo();
            if (alarmConfig == null) {
                return ARESULT.E_FAIL;
            }

            mAlarmNoticeConfig = alarmConfig;

            return ARESULT.S_OK;
        }

        /// <summary>
        /// 获取告警通知配置信息
        /// </summary>
        public ARESULT GetAlarmNoticeConfigInfo(ref string str)
        {
            str = JsonUtils.ObjectToJson<AlarmNoticeConfig>(mAlarmNoticeConfig);
            if (str != null)
                return ARESULT.S_OK;
            else
                return ARESULT.E_FAIL;
        }

        /// <summary>
        /// 更新告警通知配置信息
        /// </summary>
        public ARESULT UpdateAlarmNoticeConfig(
            string data)
        {
            AlarmNoticeConfig config = JsonUtils.ObjectFromJson<AlarmNoticeConfig>(data);
            if (config == null)
                return ARESULT.E_FAIL;

            ARESULT ret = AlarmNoticeConfigDAO.UpdateAlarmNoticeConfigInfo(
                config.mSendUser,
                config.mIsAlarmSend,
                config.mIsHourSend,
                config.mIsRegularTimeSend,
                config.mRegularTime,
                config.mIsAutoReply,
                config.mIsSelectionRecord,
                config.mIsGroupSelectionRecord);
            if (ARESULT.AFAILED(ret))
                return ARESULT.E_FAIL;

            lock (mAlarmNoticeConfig) {
                mAlarmNoticeConfig.mSendUser = config.mSendUser;
                mAlarmNoticeConfig.mIsAlarmSend = config.mIsAlarmSend;
                mAlarmNoticeConfig.mIsHourSend = config.mIsHourSend;
                mAlarmNoticeConfig.mIsRegularTimeSend = config.mIsRegularTimeSend;
                mAlarmNoticeConfig.mRegularTime = config.mRegularTime;
                mAlarmNoticeConfig.mIsAutoReply = config.mIsAutoReply;
                mAlarmNoticeConfig.mIsSelectionRecord = config.mIsSelectionRecord;
                mAlarmNoticeConfig.mIsGroupSelectionRecord = config.mIsGroupSelectionRecord;
                mRealtimeNoticeWorker.SetConfig(mAlarmNoticeConfig.mIsHourSend,
                    mAlarmNoticeConfig.mIsRegularTimeSend, mAlarmNoticeConfig.mRegularTime);
            }

            return ARESULT.S_OK;
        }

        #endregion

        #region 红外参数

        /// <summary>
        /// 加载红外参数
        /// </summary>
        /// <returns></returns>
        public ARESULT LoadIRParam()
        {
            mIRParam = IRParamDAO.GetIRParam();
            if (mIRParam == null)
                return ARESULT.E_FAIL;

            return ARESULT.S_OK;
        }

        /// <summary>
        /// 获取红外参数
        /// </summary>
        /// <returns></returns>
        public ARESULT GetIRParam(ref string str)
        {
            str = JsonUtils.ObjectToJson(mIRParam);
            if (str != null)
                return ARESULT.S_OK;
            else
                return ARESULT.E_FAIL;
        }

        /// <summary>
        /// 更新红外参数
        /// </summary>
        /// <returns></returns>
        public ARESULT UpateIRParam(string data)
        {
            IRParam param = JsonUtils.ObjectFromJson<IRParam>(data);
            if (param == null)
                return ARESULT.E_FAIL;

            int id = 0;
            ARESULT ret = IRParamDAO.UpateIRParam(param.mEmissivity,
                param.mReflectedTemperature,
                param.mTransmission,
                param.mAtmosphericTemperature,
                param.mRelativeHumidity,
                param.mDistance,
                ref id);

            if (ARESULT.AFAILED(ret))
                return ARESULT.E_FAIL;

            lock (mIRParam) {
                mIRParam.mAtmosphericTemperature = param.mAtmosphericTemperature;
                mIRParam.mDistance = param.mDistance;
                mIRParam.mEmissivity = param.mEmissivity;
                mIRParam.mId = id;
                mIRParam.mReflectedTemperature = param.mReflectedTemperature;
                mIRParam.mRelativeHumidity = param.mRelativeHumidity;
                mIRParam.mTransmission = param.mTransmission;
            }

            SetEmissivity(param.mEmissivity);
            SetReflectedTemperature(param.mReflectedTemperature);
            SetTransmission(param.mTransmission);
            SetAtmosphericTemperature(param.mAtmosphericTemperature);
            SetRelativeHumidity(param.mRelativeHumidity);
            SetDistance(param.mDistance);

            return ARESULT.S_OK;
        }

        #endregion

        #region 实时通知

        /// <summary>
        /// 自动回复回调
        /// </summary>
        private void OnAutoReplyShortMessageCallback(string phone)
        {
            if (!mAlarmNoticeConfig.mIsAutoReply)
                return;

            bool hasFind = false;
            float maxTemp = 0.0f;
            float minTemp = 0.0f;
            float avgTemp = 0.0f;

            lock (mSelections) {
                foreach (Selection item in mSelections) {
                    if (item.mIsGlobalSelection) {
                        hasFind = true;
                        maxTemp = item.mTemperatureData.mMaxTemperature;
                        minTemp = item.mTemperatureData.mMinTemperature;
                        avgTemp = item.mTemperatureData.mAvgTemperature;
                    }
                }
            }

            if (!hasFind)
                return;

            string msg = string.Format("最高温:{0:F2}℃\n", maxTemp)
                + string.Format("最低温:{0:F2}℃\n", minTemp)
                + string.Format("平均温:{0:F2}℃\n", avgTemp)
                + string.Format("时间:{0}\n", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            SmsServiceWorker.Instance.SendMessage(new List<string> { phone }, msg);
        }

        /// <summary>
        /// 定时/定点发送短信回调
        /// </summary>
        private ARESULT SendShortMessageCallback()
        {
            if (!mAlarmNoticeConfig.mIsHourSend && !mAlarmNoticeConfig.mIsRegularTimeSend)
                return ARESULT.E_FAIL;

            bool hasFind = false;
            float maxTemp = 0.0f;
            float minTemp = 0.0f;
            float avgTemp = 0.0f;

            lock (mSelections) {
                foreach (Selection item in mSelections) {
                    if (item.mIsGlobalSelection) {
                        hasFind = true;
                        maxTemp = item.mTemperatureData.mMaxTemperature;
                        minTemp = item.mTemperatureData.mMinTemperature;
                        avgTemp = item.mTemperatureData.mAvgTemperature;
                    }
                }
            }

            if (!hasFind)
                return ARESULT.E_FAIL;

            string msg = string.Format("最高温:{0:F2}℃\n", maxTemp)
                + string.Format("最低温:{0:F2}℃\n", minTemp)
                + string.Format("平均温:{0:F2}℃\n", avgTemp)
                + string.Format("时间:{0}\n", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            SmsServiceWorker.Instance.SendMessage(mAlarmNoticeConfig.mSendUser, msg);

            return ARESULT.S_OK;
        }

        #endregion

        #region 关于信息

        /// <summary>
        /// 获取关于信息
        /// </summary>
        /// <returns></returns>
        public ARESULT LoadDeviceInfo()
        {
            DeviceInfo infoList = DeviceDAO.GetDeviceInfo();
            if (infoList == null)
                return ARESULT.E_FAIL;

            mDeviceInfo = infoList;
            return ARESULT.S_OK;
        }

        /// <summary>
        /// 获取关于信息
        /// </summary>
        /// <returns></returns>
        public ARESULT GetDeviceInfo(ref string str)
        {
            str = JsonUtils.ObjectToJson<DeviceInfo>(mDeviceInfo);
            return (str != null ? ARESULT.S_OK : ARESULT.E_FAIL);
        }

        #endregion

        /// <summary>
        /// 获取设备序列号
        /// </summary>
        /// <returns></returns>
        public string GetDeviceSN()
        {
            return mDeviceInfo?.mSerialNumber;
        }

        #region 修正

        /// <summary>
        /// 修正告警记录
        /// </summary>
        /// <returns>是否成功</returns>
        private ARESULT FixAlarmInfo()
        {
            return AlarmDAO.UpdateAlarmInfo(mCell.mCellId);
        }

        /// <summary>
        /// 修正选区
        /// </summary>
        /// <returns></returns>
        private ARESULT FixSelection()
        {
            string data = SelectionDAO.GetGlobalSelectionData(mCell.mCellId);
            if (data == null)
                return ARESULT.S_OK;

            RectangleSelection selectioin = JsonUtils.ObjectFromJson<RectangleSelection>(data);
            if (selectioin == null)
                return ARESULT.E_FAIL;

            if ((selectioin.mRectangle.Width == mCell.mIRCameraWidth)
                && (selectioin.mRectangle.Height == mCell.mIRCameraHeight)) {
                if (ARESULT.AFAILED(SelectionDAO.FixSelectionList(mCell.mCellId)))
                    return ARESULT.E_FAIL;

                return ARESULT.S_OK;
            }

            // 分辨率不匹配,清空所有选区和选区组
            if (ARESULT.AFAILED(SelectionDAO.RemoveAllSelection(mCell.mCellId)))
                return ARESULT.E_FAIL;

            if (ARESULT.AFAILED(GroupSelectionDAO.RemoveAllGroupSelection(mCell.mCellId)))
                return ARESULT.E_FAIL;

            return ARESULT.S_OK;
        }

        #endregion

        /// <summary>
        /// 获取红外摄像头设备
        /// </summary>
        /// <returns>红外摄像头设备</returns>
        private IDevice GetIrCameraDevice()
        {
            lock (mDeviceList) {
                return mDeviceList.First(device => device.GetDeviceType() == DeviceType.IrCamera);
            }
        }
    }
}
