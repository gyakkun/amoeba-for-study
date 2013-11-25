﻿using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Library;
using Library.Net.Amoeba;
using Library.Io;

namespace Amoeba.Windows
{
    [DataContract(Name = "StoreInfo", Namespace = "http://Amoeba/Windows")]
    class StoreTreeItem : IDeepCloneable<StoreTreeItem>, IThisLock
    {
        private string _signature;
        private BoxCollection _boxes;
        private bool _isExpanded = true;
        private bool _isUpdated;

        private volatile object _thisLock;
        private static readonly object _initializeLock = new object();

        [DataMember(Name = "UploadSignature")]
        public string Signature
        {
            get
            {
                lock (this.ThisLock)
                {
                    return _signature;
                }
            }
            set
            {
                lock (this.ThisLock)
                {
                    _signature = value;
                }
            }
        }

        [DataMember(Name = "Boxes")]
        public BoxCollection Boxes
        {
            get
            {
                lock (this.ThisLock)
                {
                    if (_boxes == null)
                        _boxes = new BoxCollection();

                    return _boxes;
                }
            }
        }

        [DataMember(Name = "IsExpanded")]
        public bool IsExpanded
        {
            get
            {
                lock (this.ThisLock)
                {
                    return _isExpanded;
                }
            }
            set
            {
                lock (this.ThisLock)
                {
                    _isExpanded = value;
                }
            }
        }

        [DataMember(Name = "IsUpdated")]
        public bool IsUpdated
        {
            get
            {
                lock (this.ThisLock)
                {
                    return _isUpdated;
                }
            }
            set
            {
                lock (this.ThisLock)
                {
                    _isUpdated = value;
                }
            }
        }

        #region IDeepClone<StoreTreeItem>

        public StoreTreeItem DeepClone()
        {
            lock (this.ThisLock)
            {
                var ds = new DataContractSerializer(typeof(StoreTreeItem));

                using (BufferStream stream = new BufferStream(BufferManager.Instance))
                {
                    using (XmlDictionaryWriter textDictionaryWriter = XmlDictionaryWriter.CreateTextWriter(stream, new UTF8Encoding(false), false))
                    {
                        ds.WriteObject(textDictionaryWriter, this);
                    }

                    stream.Position = 0;

                    using (XmlDictionaryReader textDictionaryReader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max))
                    {
                        return (StoreTreeItem)ds.ReadObject(textDictionaryReader);
                    }
                }
            }
        }

        #endregion

        #region IThisLock

        public object ThisLock
        {
            get
            {
                if (_thisLock == null)
                {
                    lock (_initializeLock)
                    {
                        if (_thisLock == null)
                        {
                            _thisLock = new object();
                        }
                    }
                }

                return _thisLock;
            }
        }

        #endregion
    }
}
