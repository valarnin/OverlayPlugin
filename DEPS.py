# If you're familiar with Chromium or other Google projects:
#   Sorry, this file actually has nothing to do with gclient.
#   I just borrowed the basic concept and naming scheme.

deps = {
    'ACT': {
        'url': 'https://github.com/EQAditu/AdvancedCombatTracker/releases/download/3.4.9.271/ACTv3.zip',
        'dest': 'Thirdparty/ACT',
        'strip': 0,
        'hash': ['sha256', 'eb5dc7074c32a534d4e80e0248976c9499c377b61892be2ae85ad631e5af6589'],
    },
    'FFXIV_ACT_Plugin': {
        'url': 'https://github.com/ravahn/FFXIV_ACT_Plugin/raw/master/Releases/FFXIV_ACT_Plugin_SDK_2.0.6.1.zip',
        'dest': 'Thirdparty/FFXIV_ACT_Plugin',
        'strip': 0,
        'hash': ['sha256', 'ed98fa01ec2c7ed2ac843fa8ea2eec1d66f86fad4c6864de4d73d5cfbf163dce'],
    },
    'FFXIVClientStructs': {
        'url': 'https://github.com/aers/FFXIVClientStructs/archive/22eb9598a3fc6572c44ce1fd1ad38b5545e35b66.zip',
        'dest': 'OverlayPlugin.Core/Thirdparty/FFXIVClientStructs/Base/Global',
        'strip': 1,
        'hash': ['sha256', '912c565503d5af4cd037e39f147fc2abb987dbe925a10a6611bcca987e558de6'],
    },
}
