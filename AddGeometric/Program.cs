﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;

namespace AddEntityToDocument
{
    public class Program
    {
        [CommandMethod("AddLine")]
        public void AddLine()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Line line = new Line(new Point3d(0, 0, 0), new Point3d(100, 100, 0));
            document.AddEntityToModelSpace(line);
        }
    }

    public static class AddEntity
    {
        public static void AddEntityToModelSpace(this Document doc, params Entity[] ents)
        {
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                foreach (var ent in ents)
                {
                    btr.AppendEntity(ent);
                    trans.AddNewlyCreatedDBObject(ent, true);
                }
                trans.Commit();
            }
        }

        public static Entity GetEntityByObjectId(this ObjectId id)
        {
            Entity ent = null;
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                ent = id.GetObject(OpenMode.ForRead) as Entity;
                trans.Commit();
            }
            return ent;
        }

        public static Entity[] GetEntitysByObjectIds(this ObjectId[] ids)
        {
            Entity[] ents = new Entity[ids.Length];
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                for(int i=0;i<ids.Length;i++)
                {
                    ents[i] = ids[i].GetObject(OpenMode.ForWrite) as Entity;
                }
                trans.Commit();
            }
            return ents;
        }

    }
}
