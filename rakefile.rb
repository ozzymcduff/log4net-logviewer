$dir = File.join(File.dirname(__FILE__),'src')
$nuget = File.join(File.dirname(__FILE__),'nuget')

require 'albacore'
require 'nuget_helper'

desc "build using msbuild"
build :build do |msb|
  msb.prop :configuration, :Debug
  msb.target = [:Rebuild]
  msb.logging = 'minimal'
  if NugetHelper.os == :windows
    msb.sln =File.join($dir, "LogViewer.sln")
  else
    msb.sln =File.join($dir, "LogViewer.Core.sln")
  end
end

desc "test using console"
test_runner :test => [:build] do |runner|
  runner.exe = NugetHelper.xunit_path
  files = Dir.glob(File.join($dir,"*Tests","bin","**","*Tests.dll"))
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
  cd $nuget do
    NugetHelper.exec "pack log4net-logviewer.nuspec"
  end
end

desc "Install missing NuGet packages."
task :install_packages do
  cd $dir do
    NugetHelper.exec("restore LogViewer.sln")
  end
end

task :clean_packages do
  rm_r(File.join($nuget,"tools/"))
  rm_r(File.join($nuget,"lib/"))
end

