using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Omnius.Wpf;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Amoeba.Interface
{
    class ChatThreadViewModel : TreeViewModelBase
    {
        private CompositeDisposable _disposable = new CompositeDisposable();
        private volatile bool _disposed;

        public ChatThreadInfo Model { get; private set; }

        public ChatThreadViewModel(TreeViewModelBase parent, ChatThreadInfo model)
            : base(parent)
        {
            this.Model = model;

            this.IsSelected = new ReactiveProperty<bool>().AddTo(_disposable);
            this.Name = model.ObserveProperty(n => n.Tag).Select(n => MessageUtils.ToString(n)).ToReactiveProperty().AddTo(_disposable);
        }

        public override string DragFormat { get { return "Amoeba_Chat"; } }

        public override bool TryAdd(object value)
        {
            return false;
        }

        public override bool TryRemove(object value)
        {
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (disposing)
            {
                _disposable.Dispose();
            }
        }
    }
}
