import { Component, OnInit } from '@angular/core';
import {FileSystemDirectoryEntry, FileSystemEntry, FileSystemFileEntry, NgxFileDropEntry} from 'ngx-file-drop';
import {ExplorerDirectory, ExplorerFile} from '../../../model/cdn/ExplorerObject';
import {Subject} from 'rxjs';
import {Router} from '@angular/router';
import {CdnAdapterService} from '../../../services/cdn/cdn-adapter.service';
import {PathHelper} from '../../../services/PathHelper';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-storage-import',
  templateUrl: './storage-import.component.html',
  styleUrls: ['./storage-import.component.scss']
})
export class StorageImportComponent implements OnInit {

  private directoriesSubject = new Subject<ExplorerDirectory[]>();
  private filesSubject = new Subject<ExplorerFile[]>();

  public directoriesObservable = this.directoriesSubject.asObservable();
  public filesObservable = this.filesSubject.asObservable();

  public path = '/';
  public files: FileSystemFileEntry[] = [];
  public filesView: ExplorerFile[] = [];

  constructor(
    public router: Router,
    public cdnAdapter: CdnAdapterService,
    public snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
  }

  filesSelected(files: NgxFileDropEntry[]): void {
    files.forEach(f => {
      if (f.fileEntry.isFile) {
        this.readFile(f.fileEntry);
      } else {
        this.readDirectory(f.fileEntry).forEach(e => this.readFile(e));
      }
    });

    this.filesSubject.next(this.filesView);
  }

  readFile(file: FileSystemEntry): void {
    const fileEntry = file as FileSystemFileEntry;
    this.files.push(fileEntry);
    fileEntry.file(f => {
      // @ts-ignore
      const fullPath = fileEntry.fullPath;
      this.filesView.push(new ExplorerFile(fullPath ?? file.name, fullPath ?? file.name,
        f.type, f.size, f.lastModified.toString(), f.lastModified.toString()));
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
    this.files.forEach(f => {
      // @ts-ignore
      const fullPath = f.fullPath ?? f.name;
      f.file((stream: File) => {
        console.log(stream);
        stream.arrayBuffer().then(buffer => {
          const uri = PathHelper.getPath(this.path, fullPath);
          let type = stream.type;
          if (!type || type.trim().length === 0) {
            type = 'application/octet-stream';
          }
          this.cdnAdapter.store(uri, type, buffer).subscribe(() => {
            this.snackBar.open('File "' + uri + '" uploaded.', null, {duration: 2000});
            const index = this.filesView.findIndex(r => r.path === uri);
            this.filesView.splice(index, 1);
            this.filesSubject.next(this.filesView);
          }, () => {
            this.snackBar.open('File upload "' + uri + '" failed.', null, {duration: 2000});
          });
        });
      });
    });
  }
}
