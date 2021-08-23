# SaveWindowsWallpaper

Windows10 のロック画面に表示されたことのある WindowsSpotlight の壁紙を保存するプログラム。

Gist で管理していた [SaveWindowsWallpaper2](https://gist.github.com/mystasly48/ccd40ca5379be82a768d5196a1345de1) の修正版です。

## 使い方

1. Windows の設定から、ロック画面の背景を「Windows スポットライト」に設定してください。この設定は、新しい背景画像の収集に必須です。

1. `csc.exe` などの C# コンパイラを用いて、`SaveWindowsWallpaper.cs` をコンパイルしてください。なお、C# 6.0 以降が想定されています。

1. `SaveWindowsWallpaper2.exe` を適当な場所で実行してください。コンソールが立ち上がり、処理が行われます。

1. ピクチャフォルダに `SaveWindowsWallpaper` という名前のフォルダが作成されます。`Horizontal` フォルダには横長の画像、`Vertical` フォルダには縦長の画像が保存されています。

毎日実行することが好ましいため、スタートアップフォルダの `shell:startup` などに `SaveWindowsWallpaper2.exe` のショートカットを配置しておくと良いでしょう。毎日増えていくはずです。

また、`SaveWindowsWallpaper/Horizontal` フォルダを背景のスライドショーアルバムに設定しておくと最高です。自動で増える背景に自動で切り替わるため、背景画像に飽きません。

### 出力の確認

検索対象のファイルが存在しなかった場合に、コンソール画面に `Files not found.` と表示されます。これはエラーではなく、新しい背景画像が生成されていなかったことを意味します。

対象のファイルが存在した場合には、以下のログを出力しながら処理を行い、完了すると `Completed.` と表示されます。

- `Moved: Horizontal` 横長画像のため Horizontal フォルダに移動しました。
- `Moved: Vertical` 縦長画像のため Vertical フォルダに移動しました。
- `Deleted: Duplicate` 既にコピーしたことのある画像のため削除しました。
- `Deleted: Too small` 画像が 1920x1080 の解像度を満たさなかったため削除しました。（この場合、基本的に意味不明なアイコンです）
- `Deleted: Not image` 画像ではなかったため削除しました。

*※ `Moved` の処理では、元ファイルを残さない「切り取り(移動)」をしています。また、`Deleted` においても、元ファイルを残さない「削除」をしています。*

### 関連フォルダ・ファイルについて

このプログラムは、以下のフォルダ・ファイルにアクセスしています。

- 元画像の保存フォルダです。移動や削除などを行います。
`%LocalApplicationData%\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets\`

- 横長の画像を保存します。  
`%MyPictures%\SaveWindowsWallpaper\Horizontal\`

- 縦長の画像を保存します。  
`%MyPictures%\SaveWindowsWallpaper\Vertical\`

- 画像重複防止用の設定ファイルを保存します。
`%MyPictures%\SaveWindowsWallpaper\Settings.xml`

もしも元画像の保存フォルダが存在しないようなエラーが発生した場合には、`%LocalApplicationData%\Packages\` にアクセスし、それっぽいフォルダを探してください。フォルダ名のパターン等、詳細については現在調査中です。

`%LocalApplicationData%` はユーザにより異なることがありますが、基本的には `C:\Users\[username]\AppData\Local\` です。

`%MyPictures%` もユーザにより異なることがありますが、基本的には `C:\Users\[username]\Pictures\` です。

`Settings.xml` は以前に移動した画像のハッシュ値を保存しているため、削除されると保存画像に重複が生じます。

`[username]` はパソコンのユーザ名です。

### 検討中

- 画像の重複判定にハッシュ値を利用していますが、解像度等の違いで大きく変わってしまうため、類似度で比較する方法を検討中です。
- 現在、一部でモノクロの背景画像が採用されています。個人的に嫌いなため、モノクロ画像を排除する設定を検討中です。
- 一定の比率で動物・昆虫・植物の背景画像が採用されています。個人的に不快なため、これらの画像を排除する設定を検討中です。
- 現在は Windows スポットライトの背景画像を保存することに焦点を当てていますが、WEB 上の情報も含めた全背景画像の収集を目標にすることも検討中です。別途のプロジェクトになる可能性もあります。
