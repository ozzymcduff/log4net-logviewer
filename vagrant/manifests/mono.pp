
package { "python-software-properties":
    ensure => installed
}

exec { "src mono-3.2": #https://launchpad.net/~v-kukol/+archive/mono-3.2
    command => "/usr/bin/add-apt-repository ppa:v-kukol/mono-3.2",
}

exec { "apt-get update":
    command => "/usr/bin/apt-get update",
}
 
package { "mono-devel":
    ensure => installed,
    require => [Exec["src mono-3.2"],Exec["apt-get update"]],
}

package { "nunit-console":
    ensure => installed,
    require => Package["mono-devel"],
}
