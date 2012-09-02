require 'albacore'

namespace :mono do
  desc "build on mono"
  xbuild :build do |msb|
    msb.properties :configuration => :Debug
    msb.targets :rebuild
    msb.verbosity = 'quiet'
    msb.solution = "LogTail.sln"
  end
end

