require 'albacore'
desc "Nuget pack"
task :nugetpack => ["logviewer:ms:nugetpack"]
namespace :logviewer do
  $dir = File.join(File.dirname(__FILE__),'src')
  $nuget = File.join(File.dirname(__FILE__),'nuget')
  namespace :ms do
    desc "build using msbuild"
    msbuild :build do |msb|
      msb.properties :configuration => :Debug
      msb.targets :Clean, :Rebuild
      msb.verbosity = 'quiet'
      msb.solution = File.join($dir, "LogViewer.sln")
    end
    desc "test using nunit console"
    nunit :test => :build do |nunit|
      nunit.command = File.join($dir,"packages/NUnit.2.5.9.10348/Tools/nunit-console-x86.exe")
      nunit.assemblies File.join($dir,"Tests/bin/Debug/Tests.dll")
    end

    task :core_copy_to_nuspec => [:build] do
      output_directory_lib = File.join($nuget,"lib/40/")
      mkdir_p output_directory_lib
      cp Dir.glob(File.join($dir, "Core/bin/Debug/log4net-logviewer.core.dll")), output_directory_lib
    end

    desc "create the nuget package"
    task :nugetpack => [:core_nugetpack]

    task :core_nugetpack => [:core_copy_to_nuspec, :runners_copy_to_nuspec] do |nuget|
      cd File.join($nuget) do
        sh "..\\src\\.nuget\\NuGet.exe pack log4net-logviewer.nuspec"
      end
    end

    task :runners_copy_to_nuspec => [:build] do
      output_directory_lib = File.join($nuget,"tools/")
      mkdir_p output_directory_lib
      ['LogTail', 'LogViewer'].each{ |project|
        cp Dir.glob(File.join($dir,"#{project}/bin/Debug/#{project}.exe")), output_directory_lib
      }
      cp Dir.glob(File.join($dir,"LogTail/bin/Debug/log4net.dll")), output_directory_lib
end

    task :clean_packages do
      rm_r(File.join($nuget,"tools/"))
      rm_r(File.join($nuget,"lib/"))
    end

    desc "install missing nuget packages"
    task :packages do
      FileList["**/packages.config"].each { |filepath|
        sh ".\\src\\.nuget\\NuGet.exe i #{filepath} -o ./packages"
      }
    end
  end

end