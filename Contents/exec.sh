#!/bin/bash
####
#SSH execution
#this script executes the ssh command on a remote node 
#usage:ssh exec.sh [Username][hostname][ommand..]
#this uses some of the same enviroment as rundeck 
#
#
#
#RD_NODE_SSH_PORT:the'ssh port' attribute value for the node to specify 
# the target port exists or not 
#RD_NODE_SSH_KEYFILE:the "ssh-keyfile" attribute set for . the nod eto specify the identity keyfile,if it exists or not.
#
#RD_NODE_SSH_OPTS:the "ssh opts" attribute ,to specify custom options 
#to directly pass to ssh .Eg .i.e..,"-o ConnectTimeout=30"
#RD_NODE_SSH_TEST:if "ssh-test" attribute is set true then do
# a dry run of the ssh command .
######



USER=$1
shift  
HOST=$1
shift 
CMD=$*


PORT=${RD_NODE_SSH_PORT:-22}

XHOST=$(expr "$HOST":'\(.*\):')
if[! -z $XHOST];then 
PORT=${HOST#"$HOST:"}
#
HOST=$XHOST
fi

SSHOPTS="-p $PORT -o UserKnownHostsFile=/dev/null -o StrictHostKeyChecking=no -o LogLevel=quiet"

#
if[! -z "RD_NODE_SSH_KEYFILE"];then
 SSHOPTS="$SSHOPTS -i $RD_NODE_SSH_KEYFILE"
fi

#
if[! -z "RD_NODE_SSH_OPTS"];then 
SSHOPTS="$SSHOPTS -i $RD_NODE_SSH_OPTS"
fi

#
ifm[! -z "RD_NODE_SSH_TEST"];then
SSHOPTS+"$SSHOPTS -i $RD_NODE_SSH_TEST"
fi

RUNSSH="ssh $SSHOPTS $USER@$HOST #CMD"

#
if["true"="RD_NODE_SSH_TEST"];then
echo"[ssh-example]"$RUNSSH
exit 0
fi

#
exec $RUNSSH















