require 'albacore'
namespace :logviewer do
  dir = File.dirname(__FILE__)
  
  namespace :ms do
    desc "build using msbuild"
    msbuild :build do |msb|
      msb.properties :configuration => :Debug
      msb.targets :Clean, :Rebuild
      msb.verbosity = 'quiet'
      msb.solution = File.join(dir, "LogViewer.sln")
    end
    desc "test using nunit console"
    nunit :test => :build do |nunit|
      nunit.command = File.join(dir,"packages/NUnit.2.5.9.10348/Tools/nunit-console-x86.exe")
      nunit.assemblies File.join(dir,"Tests/bin/Debug/Tests.dll")
    end

    task :core_copy_to_nuspec => [:build] do
      output_directory_lib = File.join(dir,"nuget/lib/40/")
      mkdir_p output_directory_lib
      cp Dir.glob("./Core/bin/Debug/log4net-logviewer.core.dll"), output_directory_lib
    end

    desc "create the nuget package"
    task :nugetpack => [:core_nugetpack]

    task :core_nugetpack => [:core_copy_to_nuspec, :runners_copy_to_nuspec] do |nuget|
      cd File.join(dir,"nuget") do
        sh "..\\.nuget\\NuGet.exe pack log4net-logviewer.nuspec"
      end
    end

    task :runners_copy_to_nuspec => [:build] do
      output_directory_lib = File.join(dir,"nuget/tools/")
      mkdir_p output_directory_lib
      ['LogTail', 'LogViewer'].each{ |project|
        cp Dir.glob("./#{project}/bin/Debug/#{project}.exe"), output_directory_lib
      }
    end

    desc "install missing nuget packages"
    task :packages do
      FileList["**/packages.config"].each { |filepath|
        sh ".\\.nuget\\NuGet.exe i #{filepath} -o ./packages"
      }
    end
  end

end