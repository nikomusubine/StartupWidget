# StartupWidget
## アプリの機能
### 目的
ソフトウェアを起動するときに、デスクトップでマウスを操作するのも、スタートメニューから探すのも面倒くさい。そのため、自作アプリで即座に呼び出し可能にする。ソフトはあらかじめ登録する必要がある。
### 主要機能
- メインウィンドウ
このアプリの主ウィンドウ。必ずこれが起動する。ここからも外部ソフトを起動できる。ウィジェットを呼ぶ、設定ウィンドウを呼ぶ、などが可能
- ウィジェット
ショートカットキーで呼び出し可能。ショートカットキーはユーザーが設定可能。
- 設定ウィンドウ
様々な設定を行う。設定クラスの内容を参照して、自動で設定項目を構成する。
- 編集ウィンドウ
登録された外部ソフトを編集する

## 技術スタック・ライブラリ
- C# .NET 10.0 WPF
- CommunityToolkit.Mvvm

## アーキテクチャルール
アーキテクチャルールはMVVMを採用。
- View: UI、ロジックを持たない
- ViewModel: CommunityToolkitを活用
- Service: Modelの変更等を仲介する。
- Model: 様々なリソース本体となるクラス。

## ディレクトリ構成
- `Atttributes/`: Modelで用いるAttribute。主に設定の自動構成で用いる。
  - FloatBoundsAttribute.cs: float型の範囲を指定するAttribute
  - IntBoundsAttribute.cs: int型の範囲を指定するAttribute
  - SettingElementIgnoreAttribute.cs: 設定ファイルで表示させたくない項目に付ける
  - StringOptionAttribute.cs: string型に対して指定する。自由入力ではなくComboBoxを使用したい場合に付与する
- `Behaviours/`: ウィンドウやコントロールのBehaviour
  - ShortcutKeySetBehaviour.cs: ショートカットキーを設定するためのBehaviour。TextBoxに付ける
  - WindowDragMoveBehaviour.cs: WindowStyle=Noneのウィンドウの任意の位置をドラッグして移動可能にする。Widgetに使用 
- `Controls/`: 主に設定ウィンドウ向け。自作コントロールを配置する
  - FloatNumericBox.cs: float型の入力を受け付ける。TextBoxを継承。最小値、最大値の設定に対応
  - IntNumericBox.cs: int型の入力を受け付ける。TextBoxを継承。最小値、最大値の設定に対応
- `Languages/`: 多言語化するための言語パック(xml)を配置する
- `Messages/`: Messageを配置する
  - Messages.cs: アプリで使うメッセージが入っている
- `Models/`: Model
  - `Settings/`: 設定項目のModel
    - GeneralSettings.cs: 全ウィンドウ共通の設定を管理する
    - MainWindowSettings.cs: メインウィンドウ用の設定を管理する
    - WidgetSettings.cs: ウィジェット用の設定を管理する
  - `Types/`: データ型が配置される
    - ExecutableProcess.cs: 外部ソフトの情報のModel
      - Name: string, 表示名
      - FilePath: string, ファイルのパス
      - Arguments: string, 実行時のコマンドライン引数
      - WorkingDirectory: string, 作業ディレクトリ
      - RunasAdmin: bool, 常に管理者権限で起動するかどうか。trueでは、盾アイコンがUIで表示される
      - IconFilePath: string, カスタムアイコンのパス。空になりうる
    - SerializableFont.cs: 設定Modelで使用する。フォント名とサイズを保存する
    - ShortcutKeys.cs: 設定Modelで使用する。ショートカットキーを保存する。
- `Services/`: サービス
  - ExecutableIconImage.cs: 表示させるべきアイコンを取得する。
  - ExecutableProcessService.cs: 実行ファイル群を管理する
  - HotkeyRegisterService.cs: Win32APIでショートカットキーを登録
  - LanguageService.cs: 言語パックのロードおよびcsコードからの言語パック内容の取得
  - PersistenceService: ExecutableProcessや設定modelを保存する
  - SettingsService: 3つの設定項目を一元管理する
- `Templates/`: UIで使用するテンプレート
  - ProcessItemViewModelTemplate.xaml: Listview上に登録ソフトをどう表示するかのテンプレート
  - SettingsElementTemplate.xaml: 様々なデータ型の設定項目の表示を管理するテンプレート
- `ViewModels/`:
  - `Items/`: 一つ一つのアイテム(ExecutableProcessや設定項目)に関する
    - ColorSettingItemViewModel.cs
    - FilePathSettingItemViewModel.cs
    - FontSettingItemViewModel.cs
    - LimitedSettingItemViewModel.cs
    - ProcessItemViewModel.cs: ExecutableProcesのアイコンはここが保持する。
    - SettingItemViewModel: 
  - EditWindowViewModel.cs:
  - MainWIndowViewModel.cs;
  - ProcessViewModel.cs: プロセス群をUI表示させることを管理。ProcessItemViewModelを使用する。
  - SettingsWindowViewModel:
  - WidgetWindowViewModel:
- `Views/`
  - `Extensions/`: Xaml拡張
    - LangExtension: 言語パックの内容を読み取る
  - EditWindow.xaml
  - MainWindow.xaml:
  - SettingsItemTemplateSelector.cs: どの設定テンプレートを使用するかを操る
  - SettingsWindow.xaml:
  - WidgetWindow.xaml:
- App.xaml
- Constants.cs: 様々なところで使用する定数を保管。ファイルのパス等、直書きすると長くなるものを配置。
