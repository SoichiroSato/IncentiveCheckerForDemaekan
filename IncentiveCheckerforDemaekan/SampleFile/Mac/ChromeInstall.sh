#!/bin/bash
appList=`ls -1 /Applications/`
echo $appList
if ! [[ "$appList" == *Chrome.app* ]]; then
  brew=`brew install google-chrome --cask`
  echo $res
  if [[ "$brew" == *not* ]] && [[ "$brew" == *found* ]]; then
    /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install.sh)"
  fi
  brewResult=`brew install google-chrome --cask`
  echo $brewResult
  if ! [[ "$brewResult" == *already* ]] && [[ "$brewResult" == *installed* ]]; then
    brew uninstall google-chrome
  fi
  brew reinstall --cask google-chrome
fi