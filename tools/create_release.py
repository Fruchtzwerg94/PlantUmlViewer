# -*- coding: utf-8 -*-
"""
Script to help creating a release of the plugin

@author: Philipp Schmidt
"""

import hashlib
import json
import os.path
import pathlib
import zipfile


def zip_files(directory, zip_file):
    with zipfile.ZipFile(zip_file, 'w', zipfile.ZIP_DEFLATED) as zipf:
        for root, dirs, files in os.walk(directory):
            for file in files:
                zipf.write(os.path.join(root, file),
                           os.path.relpath(os.path.join(root, file), directory))

def hash_file(filename):
    sha256_hash = hashlib.sha256()
    with open(filename, 'rb') as f:
        for byte_block in iter(lambda: f.read(4096), b''):
            sha256_hash.update(byte_block)
        return sha256_hash.hexdigest()

def create_plugin_list_entry(version, architecture, hash):
    entry = {
        'folder-name': 'PlantUmlViewer',
        'display-name': 'PlantUML Viewer',
        'version': '{version}'.format(version=version),
        'npp-compatible-versions': '[8.3,]',
        'id': '{hash}'.format(hash=hash),
        'repository': 'https://github.com/Fruchtzwerg94/PlantUmlViewer/releases/download/{version}/PlantUmlViewer_v{version}_{architecture}.zip'
            .format(version=version, architecture=architecture),
        'description': 'A Notepad++ plugin to generate view and export PlantUML diagrams.',
        'author': 'Philipp Schmidt',
        'homepage': 'https://github.com/Fruchtzwerg94/PlantUmlViewer'
    }
    return json.dumps(entry, indent=4)

def main():
    input('Press any key to confirm builds were done for x86 and x64 and the PlantUML binary was added to the build directory')
    script_directory = pathlib.Path(__file__).parent.resolve()
    x86_build_directory = os.path.join(script_directory, '..', 'PlantUmlViewer', 'PlantUmlViewer', 'bin', 'Release')
    x64_build_directory = os.path.join(script_directory, '..', 'PlantUmlViewer', 'PlantUmlViewer', 'bin', 'Release-x64')
    if not os.path.isdir(x86_build_directory):
        print('x86 build directory does not exist: ' + x86_build_directory)
        return
    if not os.path.isdir(x64_build_directory):
        print('x64 build directory does not exist: ' + x64_build_directory)
        return

    version = input('Enter version: ')

    x86_file = 'PlantUmlViewer_v{version}_{architecture}.zip'.format(version=version, architecture='x86')
    x64_file = 'PlantUmlViewer_v{version}_{architecture}.zip'.format(version=version, architecture='x64')

    zip_files(x86_build_directory, x86_file)
    zip_files(x64_build_directory, x64_file)

    x86_hash = hash_file(x86_file)
    x64_hash = hash_file(x64_file)

    print(create_plugin_list_entry(version, 'x86', x86_hash))
    print()
    print(create_plugin_list_entry(version, 'x64', x64_hash))

if __name__ == '__main__':
    main()
