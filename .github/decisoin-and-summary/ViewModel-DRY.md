# 実施状況
未済

# 目的
MainWindowViewModelとWidgetWindowViewModelのロジック共通部分をDRY化したい

# 実装案
- 合成
	- ロジック共通部をSharedViewModel(仮名)に入れて、二つのViewModelはSharedViewModelへの参照を持つ
	- 元の二つのViewModelがラッパープロパティを持つ必要あり
	- SharedのMethodをCommandのメソッドから呼び出すようにすればCommandも共通化可能

- 抽象基底クラス
	- 実装が単純
	- 多重継承不可、柔軟性減

# 決定事項
合成案を採用する。

# 注意事項
- 全ラップは不要。頻繁に使うもののみでOK
- public ImageSource ShieldIcon => _shared.ShieldIcon;のように書ける
- PropertyChangedの伝播は
	- SharedComponentがObservableObjectを実装
	- ViewModelがShared.PropertyChangedを購読、自身のOnPropertyChangedを発行。

サンプルコード:

```
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Windows.Media;

public class SharedViewModel : ObservableObject
{
    private ImageSource _shieldIcon;
    public ImageSource ShieldIcon
    {
        get => _shieldIcon;
        set => SetProperty(ref _shieldIcon, value);
    }

    public SharedViewModel()
    {
        // 初期化例（実際の初期化は既存コードに合わせる）
        // ShieldIcon = ...;
    }
}

public partial class MainWindowViewModel : ObservableObject
{
    public SharedViewModel Shared { get; }

    // XAML バインド用ラッパー
    public ImageSource ShieldIcon => Shared.ShieldIcon;

    public MainWindowViewModel(SharedViewModel shared)
    {
        Shared = shared;
        // Shared のプロパティ変更を購読して、自身のラッパーに通知を伝搬する
        Shared.PropertyChanged += Shared_PropertyChanged;
    }

    private void Shared_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Shared の ShieldIcon が変わったら、自分の ShieldIcon も変わったと通知する
        if (e.PropertyName == nameof(SharedViewModel.ShieldIcon) || string.IsNullOrEmpty(e.PropertyName))
        {
            OnPropertyChanged(nameof(ShieldIcon));
        }

        // 複数のプロパティを単純に透過させたい場合（ラッパー名が同じなら）:
        // if (!string.IsNullOrEmpty(e.PropertyName)) OnPropertyChanged(e.PropertyName);
    }
}
```
