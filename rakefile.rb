desc "Nuget pack"
$dir = File.join(File.dirname(__FILE__),'src')
$nuget = File.join(File.dirname(__FILE__),'nuget')

require 'albacore'
require_relative './src/.nuget/nuget'

dir = File.dirname(__FILE__)

desc "build using msbuild"
build :build => [:install_packages] do |msb|
  msb.prop :configuration, :Debug
  msb.target = [:Rebuild]
  msb.logging = 'minimal'
  msb.sln =File.join(dir, "src", "LogViewer.sln")
end

desc "test using console"
test_runner :test => [:build] do |runner|
  runner.exe = NuGet::nunit_path
  files = [File.join(File.dirname(__FILE__),"src","LogViewer.Tests","bin","Debug","LogViewer.Tests.dll"),
    File.join(File.dirname(__FILE__),"src","Tests","bin","Debug","Tests.dll")]
  runner.files = files 
end

desc "create the nuget package"
task :nugetpack => [:core_nugetpack]

task :core_copy_to_nuspec => [:build] do
  output_directory_lib = File.join($nuget,"lib/40/")
  mkdir_p output_directory_lib
  cp Dir.glob(File.join($dir, "Core/bin/Debug/log4net-logviewer.core.dll")), output_directory_lib
end
task :runners_copy_to_nuspec => [:build] do
  output_directory_lib = File.join($nuget,"tools/")
  mkdir_p output_directory_lib
  ['LogTail', 'LogViewer'].each{ |project|
    cp Dir.glob(File.join($dir,"#{project}/bin/Debug/#{project}.exe")), output_directory_lib
  }
  cp Dir.glob(File.join($dir,"LogTail/bin/Debug/log4net.dll")), output_directory_lib
end

task :core_nugetpack => [:core_copy_to_nuspec, :runners_copy_to_nuspec] do |nuget|
  cd File.join(dir,"nuget") do
    NuGet::exec "pack log4net-logviewer.nuspec"
  end
end

desc "Install missing NuGet packages."
task :install_packages do
    NuGet::exec("restore ./src/LogViewer.sln")
end

task :clean_packages do
  rm_r(File.join($nuget,"tools/"))
  rm_r(File.join($nuget,"lib/"))
end

