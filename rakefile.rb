require 'albacore'

namespace :mono do
  desc "build on mono"
  xbuild :build do |msb|
    msb.properties :configuration => :Debug
    msb.targets :rebuild
    msb.verbosity = 'quiet'
    msb.solution = "LogTail.sln"
  end

  task :test => :build do
    # does not work for some reason 
    assemblies = "Tests.dll"
    cd "./Tests/bin/Debug" do
      sh "nunit-console4 #{assemblies}"
    end
  end
  
end

