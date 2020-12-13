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
    totalSize: 0,
    itemsInView: 0
  };
  public orderField = 'filename';
  public orderDir = 1;

  constructor(
    private cdnAdapter: CdnAdapterService,
  ) {
  }

  private static getFilename(path: string): string {
    if (path.endsWith('/')) {
      path = path.substr(0, path.length - 1);
    }

    return path.substr(path.lastIndexOf('/') + 1);
  }

  private static getPrevPath(path: string): string {
    let newPath = path;
    if (newPath.endsWith('/')) {
      newPath.substring(0, newPath.length - 1);
    }

    newPath = newPath.substring(0, newPath.lastIndexOf('/')) + '/';
    return newPath;
  }

  ngOnChanges(model: SimpleChanges) {
    this.files = [];
    this.directories = [];
    this.thisFolder = {totalCount: 0, totalSize: 0, itemsInView: 0};
    this.prevPath = PathExplorerComponent.getPrevPath(this.path);
    this.reload();
  }

  reload(): void {
    this.cdnAdapter.explore(this.path)
      .subscribe(objects => {
        objects.forEach(o => {
          const dir = (o as ExplorerDirectory);
          if (dir.totalCount) {
            dir.filename = PathExplorerComponent.getFilename(dir.path);
            this.directories.push(dir);
            this.thisFolder.totalSize += dir.totalSize;
            this.thisFolder.totalCount += dir.totalCount;
          }
          const file = (o as ExplorerFile);
          if (file.mediaType) {
            file.filename = PathExplorerComponent.getFilename(file.path);
            this.files.push(file);
            this.thisFolder.totalSize += file.size;
            this.thisFolder.totalCount++;
          }

          this.thisFolder.itemsInView++;
        });

        this.sort();
      });
  }

  private sort(): void {
    this.directories = this.directories.sort((a, b) => {
      let field = this.orderField;

      if (field === 'mediaType') { field = 'totalCount'; }
      if (field === 'size') { field = 'totalSize'; }

      return a[field] > b[field] ? this.orderDir : this.orderDir * -1;
    });
    this.files = this.files.sort((a, b) => {
      const field = this.orderField;
      return a[field] > b[field] ? this.orderDir : this.orderDir * -1;
    });

    console.log('sorted');
  }

  changePath(path: string): void {
    this.pathChanged.emit(path);
  }

  sortBy(field: string): void {
    if (this.orderField === field) {
      this.orderDir *= -1;
      this.sort();
      return;
    }

    this.orderField = field;
    this.orderDir = 1;

    this.sort();
  }
}
