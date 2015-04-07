# update-revision

A simple command-line tool that edits a C# _AssemblyInfo.cs_ file, updating the revision number in _AssemblyVersion_ to the current SVN revision number.

It might as well work on any file that includes the string _AssemblyVersion("n.n.n.n")_ (n being any number) as it will search the file for that string and replace the last number to SVN revision.

The app's project itself uses the app to update it's own version number, which could serve as an example of usage.

<br><br>
<b>Syntax:</b>

<pre><code>UpdateRevision.exe [--revert] [-f=&lt;filename&gt;] [-rd=&lt;reppath&gt;]</code></pre>

<code>-f=&lt;filename&gt;</code> : the name of the file to edit. usually it is <i>AssemblyInfo.cs</i> in the project dir or the subdir <i>Properties</i>. if not specified, it defaults to <i>AssemblyInfo.cs</i> in the current directory ("working dir"). example:<br>
<code>UpdateRevision.exe -f="c:\my project\AssemblyInfo.cs"</code>

<code>-rd=&lt;reppath&gt;</code> : the path of the repository you want to check for revision number. if not specified, it defaults to the executable path, i.e the directory where UpdateRevision.exe is. example:<br>
<code>UpdateRevision.exe -rd="c:\checkout\my project"</code>

<code>--revert </code> : set the revision number in the file to <code>*</code> . useful for post-build, if you want your <i>AssemblyInfo.cs</i> to stay the same in the repository.<br>
<br>
<br>
<b>Tips:</b><br>
If you want to set pre-build event in Visual Studio, you should add this line to your project file:<br>
<pre><code>&lt;UseHostCompilerIfAvailable&gt;FALSE&lt;/UseHostCompilerIfAvailable&gt;
<br>
</code></pre>
without it, even if the <i>AssemblyInfo.cs</i> file gets updated, the project will compile without the change, and you'll get the previous revision number.<br>
<br>
Another option would be to use <code>BeforeBuild</code> instead of <code>PreBuildEvent</code>. for example:<br>
<pre><code>&lt;Target Name="BeforeBuild"&gt;
<br>
  &lt;Exec Command="&amp;quot;$(ProjectDir)updaterevision.exe&amp;quot; &amp;quot;-f=$(ProjectDir)Properties\AssemblyInfo.cs&amp;quot; &amp;quot;-rd=$(SolutionDir)&amp;quot;" /&gt;
<br>
&lt;/Target&gt;
<br>
</code></pre>
