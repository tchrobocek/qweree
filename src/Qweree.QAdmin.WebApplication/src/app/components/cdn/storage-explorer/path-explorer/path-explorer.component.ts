import {Component, EventEmitter, Input, OnChanges, Output, SimpleChanges} from '@angular/core';
import {ExplorerDirectory, ExplorerFile} from '../../../../model/cdn/ExplorerObject';
import {CdnAdapterService} from '../../../../services/cdn/cdn-adapter.service';

@Component({
  selector: 'app-path-explorer',
  templateUrl: './path-explorer.component.html',
  styleUrls: ['./path-explorer.component.scss']
})
export class PathExplorerComponent implements OnChanges {
  @Input() public path: string;
  @Output() public pathChanged = new EventEmitter<string>();
  public directories: ExplorerDirectory[];
  public files: ExplorerFile[];
  public prevPath: string;
  public thisFolder = {
    totalCount: 0,
    totalSize: 0
  };

  constructor(
    private cdnAdapter: CdnAdapterService,
  ) {
  }

  private static getPrevPath(path: string): string {
    let newPath = path;
    if (newPath.endsWith('/')) {
      newPath.substring(0, newPath.length - 1);
    }

    newPath = newPath.substring(0, newPath.lastIndexOf('/')) + '/';
    return newPath;
  }

  ngOnChanges(model: SimpleChanges){
    this.files = [];
    this.directories = [];
    this.thisFolder = {totalCount: 0, totalSize: 0};
    this.prevPath = PathExplorerComponent.getPrevPath(this.path);
    this.reload();
  }

  reload(): void {
    this.cdnAdapter.explore(this.path)
      .subscribe(objects => {
        objects.forEach(o => {
          const dir = (o as ExplorerDirectory);
          if (dir.totalCount) {
            this.directories.push(dir);
            this.thisFolder.totalSize += dir.totalSize;
            this.thisFolder.totalCount += dir.totalCount;
          }
          const file = (o as ExplorerFile);
          if (file.id) {
            this.files.push(file);
            this.thisFolder.totalSize += file.size;
            this.thisFolder.totalCount ++;
          }
        });
      });
  }

  changePath(path: string): void {
    this.pathChanged.emit(path);
  }
}
