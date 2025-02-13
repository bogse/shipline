﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using ShipLine.Data;
using ShipLine.Models;
using Route = ShipLine.Models.DBObjects.Route;

namespace ShipLine.Repository
{
    public class RouteRepository
    {
        private readonly ApplicationDbContext _DBContext;
        public RouteRepository()
        {
            _DBContext = new ApplicationDbContext();
        }
        public RouteRepository(ApplicationDbContext dbContext)
        {
            _DBContext = dbContext;
        }

        private RouteModel MapDBObjectToModel(Route dbObject)
        {
            var model = new RouteModel();
            if(dbObject != null)
            {
                model.RouteId = dbObject.RouteId;
                model.SourcePortId = dbObject.SourcePortId;
                model.DestinationPortId = dbObject.DestinationPortId;
                model.Name = dbObject.Name;
            }
            return model;
        }
        private Route MapModelToDBObject(RouteModel model)
        {
            var dbObject = new Route();
            if(dbObject != null)
            {
                dbObject.RouteId = model.RouteId;
                dbObject.SourcePortId = model.SourcePortId;
                dbObject.DestinationPortId = model.DestinationPortId;
                dbObject.Name = model.Name;
            }
            return dbObject;
        }
        public List<RouteModel> GetAllRoutes()
        {
            var list = new List<RouteModel>();
            foreach(var dbObject in _DBContext.Routes)
            {
                list.Add(MapDBObjectToModel(dbObject));
            }
            return list;
        }
        public RouteModel GetRouteById(Guid id)
        {
            return MapDBObjectToModel(_DBContext.Routes.FirstOrDefault(x=> x.RouteId == id));
        }
        public void InsertRoute(RouteModel model)
        {
            model.RouteId = Guid.NewGuid();
            _DBContext.Routes.Add(MapModelToDBObject(model));
            _DBContext.SaveChanges();
        }
        public void UpdateRoute(RouteModel model)
        {
            var dbObject = _DBContext.Routes.FirstOrDefault(x=> x.RouteId == model.RouteId);
            if(dbObject != null)
            {
                dbObject.RouteId = model.RouteId;
                dbObject.SourcePortId = model.SourcePortId;
                dbObject.DestinationPortId = model.DestinationPortId;
                dbObject.Name = model.Name;
                _DBContext.SaveChanges();
            }
        }
        public void DeleteRoute(Guid id)
        {
            var dbObject = _DBContext.Routes.FirstOrDefault(x=> x.RouteId == id);
            if (dbObject != null)
            {
                var voyages = _DBContext.Voyages.Where(x => x.RouteId == dbObject.RouteId);
                foreach(var voyage in voyages)
                {
                    _DBContext.Voyages.Remove(voyage);
                }
                _DBContext.Routes.Remove(dbObject);
                _DBContext.SaveChanges();
            }
        }

        public List<RouteModel> GetVoyageRoute(Guid id)
        {
            var voyages = _DBContext.Voyages.Where(x => x.VoyageId == id);
            var routes = new List<RouteModel>();
            foreach(var voyage in voyages)
            {
                routes.Add(MapDBObjectToModel(_DBContext.Routes.FirstOrDefault(x => x.RouteId == voyage.RouteId)));
            }
            return routes;
        }
        public RouteModel GetRouteById(Guid sourceId, Guid destId)
        {
            return MapDBObjectToModel(_DBContext.Routes.FirstOrDefault(x => x.DestinationPortId == destId && x.SourcePortId == sourceId));
        }
    }
}
