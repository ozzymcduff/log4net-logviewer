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
  end


  namespace :mono do
    desc "build on mono"
    xbuild :build do |msb|
      msb.properties :configuration => :Debug
      msb.targets :rebuild
      msb.verbosity = 'quiet'
      msb.solution = File.join(dir, "LogTail.sln")
    end

    task :test => :build do
      # does not work for some reason 
      assemblies = "Tests.dll"
      cd File.join(dir,"Tests/bin/Debug") do
        sh "nunit-console4 #{assemblies}"
      end
    end
    
  end

end