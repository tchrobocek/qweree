import { Component, OnInit } from '@angular/core';
import {FileSystemDirectoryEntry, FileSystemEntry, FileSystemFileEntry, NgxFileDropEntry} from 'ngx-file-drop';
import {ExplorerFile} from '../../../model/cdn/ExplorerObject';

@Component({
  selector: 'app-storage-import',
  templateUrl: './storage-import.component.html',
  styleUrls: ['./storage-import.component.scss']
})
export class StorageImportComponent implements OnInit {

  public path = '';
  public files: FileSystemFileEntry[] = [];
  public filesView: ExplorerFile[] = [];

  constructor() { }

  ngOnInit(): void {
  }

  filesSelected(files: NgxFileDropEntry[]): void {
    console.log(files);
    files.forEach(f => {
      if (f.fileEntry.isFile) {
        this.readFile(f.fileEntry);
      } else {
        this.readDirectory(f.fileEntry).forEach(e => this.readFile(e));
      }
    });
  }

  readFile(file: FileSystemEntry): void {
    const fileEntry = file as FileSystemFileEntry;
    this.files.push(fileEntry);
    fileEntry.file(f => {
      // @ts-ignore
      const fullPath = fileEntry.fullPath;
      this.filesView.push(new ExplorerFile(file.name, fullPath, f.type, f.size, f.lastModified.toString(), f.lastModified.toString()));
    });
  }

  private readDirectory(directory: FileSystemEntry): FileSystemFileEntry[] {
    const dir = (directory as FileSystemDirectoryEntry);
    const reader = dir.createReader();
    const result = [];

    reader.readEntries(entries => {
      entries.forEach(e => {
        if (e.isFile) {
          result.push(e);
        } else {
          this.readDirectory(e).forEach(ee => result.push(ee));
        }
      });
    });

    return result;
  }

  uploadFiles(): void {

  }
}
