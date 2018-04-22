using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Bluetooth
{
    public delegate void BluetoothScanResultHandler(IBluetoothManager bluetoothManager, IBluetoothDevice[] bluetoothDevices);
    public delegate void BluetoothStartEndScanHandler(IBluetoothManager bluetoothManager);

    /// <summary>
    /// 蓝牙管理器
    /// </summary>
    public interface IBluetoothManager:IConnectionManager
    {
        /// <summary>
        /// 查找设备过程中找到时触发
        /// </summary>
        event BluetoothScanResultHandler onDevicesFound;

        /// <summary>
        /// 查找设备完成时触发
        /// </summary>
        event BluetoothStartEndScanHandler onDiscoveryFinished;

        /// <summary>
        /// 查找设备开始时触发
        /// </summary>
        event BluetoothStartEndScanHandler onDiscoveryStarted;

        /// <summary>
        /// 本机是否支持蓝牙
        /// </summary>
        bool Supported { get; }

        /// <summary>
        /// 开始查找设备
        /// 查找到的设备会通过onDeviceFound传出
        /// </summary>
        void SearchForBlutoothDevices();

        /// <summary>
        /// 创建蓝牙的客户端连接
        /// </summary>
        /// <returns></returns>
        IConnection CreateRfcommClientConnection(IBluetoothDevice device, Guid guid);
    }
}
